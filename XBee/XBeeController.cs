using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BinarySerialization;
using XBee.Devices;
using XBee.Frames;
using XBee.Frames.AtCommands;
using XBee.Observable;

#if WINDOWS_UWP
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
#endif

namespace XBee
{
    public class XBeeController : IDisposable
    {
        private static readonly ConcurrentDictionary<byte, TaskCompletionSource<CommandResponseFrameContent>>
            ExecuteTaskCompletionSources =
                new ConcurrentDictionary<byte, TaskCompletionSource<CommandResponseFrameContent>>();

        private static readonly ConcurrentDictionary<byte, Action<CommandResponseFrameContent>> ExecuteCallbacks =
            new ConcurrentDictionary<byte, Action<CommandResponseFrameContent>>();

#if !DEBUG
        private static readonly TimeSpan ModemResetTimeout = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan DefaultRemoteQueryTimeout = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan DefaultLocalQueryTimeout = TimeSpan.FromSeconds(1);
#else
        private static readonly TimeSpan ModemResetTimeout = TimeSpan.FromSeconds(300);
        private static readonly TimeSpan DefaultRemoteQueryTimeout = TimeSpan.FromSeconds(300);
        private static readonly TimeSpan DefaultLocalQueryTimeout = TimeSpan.FromSeconds(300);
#endif

        private static readonly TimeSpan NetworkDiscoveryTimeout = TimeSpan.FromSeconds(30);
        private readonly object _frameIdLock = new object();

        private readonly Source<SourcedData> _receivedDataSource = new Source<SourcedData>();
        private readonly Source<SourcedSample> _sampleSource = new Source<SourcedSample>();

        private SerialConnection _connection;
        private byte _frameId = byte.MinValue;
        private TaskCompletionSource<ModemStatus> _modemResetTaskCompletionSource;


        /// <summary>
        ///     Occurrs after a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> FrameMemberSerialized;

        /// <summary>
        ///     Occurrs after a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> FrameMemberDeserialized;

        /// <summary>
        ///     Occurrs before a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> FrameMemberSerializing;

        /// <summary>
        ///     Occurrs before a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> FrameMemberDeserializing;

        public HardwareVersion HardwareVersion { get; private set; }

        /// <summary>
        /// Get the local node.
        /// </summary>
        public XBeeNode Local { get; private set; }

        public bool IsOpen => _connection != null;

        public void Dispose()
        {
            Close();
            _sampleSource.Dispose();
            _receivedDataSource.Dispose();
        }

        /// <summary>
        /// Occurs when a node is discovered during network discovery.
        /// </summary>
        public event EventHandler<NodeDiscoveredEventArgs> NodeDiscovered;

        /// <summary>
        /// Occurs when data is recieved from a node.
        /// </summary>
        public event EventHandler<SourcedDataReceivedEventArgs> DataReceived;

        /// <summary>
        /// Occurs when a sample is received from a node.
        /// </summary>
        public event EventHandler<SourcedSampleReceivedEventArgs> SampleReceived;

#if WINDOWS_UWP
        /// <summary>
        /// Open a local node.
        /// </summary>
        /// <param name="device">The serial device used to talk to the controller node</param>
        /// <returns></returns>
        public async Task OpenAsync(SerialDevice device)
#else
        /// <summary>
        /// Open a local node.
        /// </summary>
        /// <param name="port">The COM port of the node</param>
        /// <param name="baudRate">The baud rate, typically 9600 or 115200 depending on the model</param>
        /// <returns></returns>
        public async Task OpenAsync(string port, int baudRate)
#endif
        {
            if (IsOpen)
                throw new InvalidOperationException("The controller is already connected, please close the existing connection.");

#if WINDOWS_UWP
            _connection = new SerialConnection(device);
#else
            _connection = new SerialConnection(port, baudRate);
#endif

            try
            {
                _connection.MemberSerializing += OnMemberSerializing;
                _connection.MemberSerialized += OnMemberSerialized;
                _connection.MemberDeserializing += OnMemberDeserializing;
                _connection.MemberDeserialized += OnMemberDeserialized;

                _connection.FrameReceived += OnFrameReceived;
                _connection.Open();

                /* Unfortunately the protocol changes based on what type of hardware we're using... */
                HardwareVersionResponseData response =
                    await ExecuteAtQueryAsync<HardwareVersionResponseData>(new HardwareVersionCommand());
                HardwareVersion = response.HardwareVersion;
                _connection.CoordinatorHardwareVersion = HardwareVersion;

                /* We want the receiver to have the hardware version in context so cycle the connection */
                _connection.Close();

                /* Stupid serial ports... */
                await
                    TaskExtensions.Retry(() => _connection.Open(), typeof (UnauthorizedAccessException),
                        TimeSpan.FromSeconds(3));

                Local = CreateNode(HardwareVersion);
            }
            catch (Exception)
            {
                Close();
                throw;
            }
        }

        /// <summary>
        /// Open a remote node.
        /// </summary>
        /// <param name="address">The address of the remote node</param>
        /// <returns>The remote node</returns>
        [Obsolete("Use GetRemoteNodeAsync")]
        public async Task<XBeeNode> GetRemoteAsync(NodeAddress address)
        {
            //TODO Get actual version for target device.  For some reason this call keeps timing out during discovery.
            //TODO Consider doing deferred operation
            //HardwareVersionResponseData version =
            //    await ExecuteAtQueryAsync<HardwareVersionResponseData>(new HardwareVersionCommand(), address);

            return await Task.FromResult(CreateNode(HardwareVersion, address));
        }

        /// <summary>
        /// Open a remote node.
        /// </summary>
        /// <param name="address">The address of the remote node</param>
        /// <returns>The remote node</returns>
        public async Task<XBeeNode> GetRemoteNodeAsync(NodeAddress address)
        {
            //TODO Get actual version for target device.  For some reason this call keeps timing out during discovery.
            //TODO Consider doing deferred operation
            //HardwareVersionResponseData version =
            //    await ExecuteAtQueryAsync<HardwareVersionResponseData>(new HardwareVersionCommand(), address);

            return await Task.FromResult(CreateNode(HardwareVersion, address));
        }

        /// <summary>
        /// Send a frame to this node.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(FrameContent frame)
        {
            await ExecuteAsync(frame, CancellationToken.None);
        }

        /// <summary>
        /// Send a frame to this node.
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(FrameContent frame, CancellationToken cancellationToken)
        {
            if(!IsOpen)
                throw new InvalidOperationException("Controller must be open to execute commands.");

            await _connection.Send(frame, cancellationToken);
        }
        
        /// <summary>
        /// Send an AT command to this node.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public async Task ExecuteAtCommand(AtCommand command, NodeAddress address = null)
        {
            if (address == null)
            {
                var atCommandFrame = new AtCommandFrameContent(command);
                await ExecuteAsync(atCommandFrame);
            }
            else
            {
                var remoteCommand = new RemoteAtCommandFrameContent(address, command);
                await ExecuteAsync(remoteCommand);
            }
        }

        /// <summary>
        /// Send a frame to this node and wait for a response.
        /// </summary>
        /// <typeparam name="TResponseFrame">The expected response type</typeparam>
        /// <param name="frame">The frame to send</param>
        /// <param name="timeout">Timeout</param>
        /// <returns>The response frame</returns>
        public async Task<TResponseFrame> ExecuteQueryAsync<TResponseFrame>(CommandFrameContent frame, TimeSpan timeout)
            where TResponseFrame : CommandResponseFrameContent
        {
            return await ExecuteQueryAsync<TResponseFrame>(frame, timeout, CancellationToken.None);
        }

        /// <summary>
        /// Send a frame to this node and wait for a response.
        /// </summary>
        /// <typeparam name="TResponseFrame">The expected response type</typeparam>
        /// <param name="frame">The frame to send</param>
        /// <param name="timeout">Timeout</param>
        /// <returns>The response frame</returns>
        public async Task<TResponseFrame> ExecuteQueryAsync<TResponseFrame>(CommandFrameContent frame, TimeSpan timeout, CancellationToken cancellationToken)
            where TResponseFrame : CommandResponseFrameContent
        {
            frame.FrameId = GetNextFrameId();

            var delayCancellationTokenSource = new CancellationTokenSource();
            Task delayTask = Task.Delay(timeout, delayCancellationTokenSource.Token);

            TaskCompletionSource<CommandResponseFrameContent> taskCompletionSource =
                ExecuteTaskCompletionSources.AddOrUpdate(frame.FrameId,
                    b => new TaskCompletionSource<CommandResponseFrameContent>(),
                    (b, source) => new TaskCompletionSource<CommandResponseFrameContent>());

            await ExecuteAsync(frame, cancellationToken);

            if (await Task.WhenAny(taskCompletionSource.Task, delayTask) == taskCompletionSource.Task)
            {
                delayCancellationTokenSource.Cancel();
                return await taskCompletionSource.Task as TResponseFrame;
            }

            throw new TimeoutException();
        }

        /// <summary>
        /// Send a frame to this node and wait for a response using a default timeout.
        /// </summary>
        /// <typeparam name="TResponseFrame">The expected response type</typeparam>
        /// <param name="frame">The frame to send</param>
        /// <returns>The response frame</returns>
        public Task<TResponseFrame> ExecuteQueryAsync<TResponseFrame>(CommandFrameContent frame)
            where TResponseFrame : CommandResponseFrameContent
        {
            return ExecuteQueryAsync<TResponseFrame>(frame, CancellationToken.None);
        }

        /// <summary>
        /// Send a frame to this node and wait for a response using a default timeout.
        /// </summary>
        /// <typeparam name="TResponseFrame">The expected response type</typeparam>
        /// <param name="frame">The frame to send</param>
        /// <param name="cancellationToken">Used to cancel the operation</param>
        /// <returns>The response frame</returns>
        public Task<TResponseFrame> ExecuteQueryAsync<TResponseFrame>(CommandFrameContent frame, CancellationToken cancellationToken)
            where TResponseFrame : CommandResponseFrameContent
        {
            return ExecuteQueryAsync<TResponseFrame>(frame, DefaultRemoteQueryTimeout, cancellationToken);
        }

        /// <summary>
        /// Send an AT command to a node and wait for a response.
        /// </summary>
        /// <typeparam name="TResponseData">The expected response data type</typeparam>
        /// <param name="command">The command to send</param>
        /// <param name="address">The address of the node.  If this is null the command will be sent to the local node.</param>
        /// <returns>The response data</returns>
        public async Task<TResponseData> ExecuteAtQueryAsync<TResponseData>(AtCommand command,
            NodeAddress address = null)
            where TResponseData : AtCommandResponseFrameData
        {
            AtCommandResponseFrameContent responseContent;

            if (address == null)
            {
                var atCommandFrame = new AtCommandFrameContent(command);
                AtCommandResponseFrame response = await ExecuteQueryAsync<AtCommandResponseFrame>(atCommandFrame, DefaultLocalQueryTimeout);
                responseContent = response.Content;
            }
            else
            {
                address.ShortAddress = address.LongAddress.IsBroadcast ? ShortAddress.Broadcast : ShortAddress.Disabled;

                var remoteCommand = new RemoteAtCommandFrameContent(address, command);
                RemoteAtCommandResponseFrame response =
                    await ExecuteQueryAsync<RemoteAtCommandResponseFrame>(remoteCommand, DefaultRemoteQueryTimeout);
                responseContent = response.Content;
            }

            if (responseContent.Status != AtCommandStatus.Success)
                throw new AtCommandException(responseContent.Status);

            return responseContent.Data as TResponseData;
        }

        /// <summary>
        /// Execute an AT command on a node without waiting for a response.
        /// </summary>
        /// <param name="command">The AT command to execute</param>
        /// <param name="address">The address of the node.  If this is null the command will be execute on the local node.</param>
        /// <returns></returns>
        public async Task ExecuteAtCommandAsync(AtCommand command, NodeAddress address = null)
        {
            await ExecuteAtQueryAsync<AtCommandResponseFrameData>(command, address);
        }

        /// <summary>
        /// Execute a command and wait for responses from multiple nodes.
        /// </summary>
        /// <typeparam name="TResponseFrame">The expected response type</typeparam>
        /// <param name="frame">The frame to send.</param>
        /// <param name="callback">This will be called when a response is recieved within the timeout period.</param>
        /// <param name="timeout">The amount of time to wait before responses will be ignored</param>
        public async Task ExecuteMultiQueryAsync<TResponseFrame>(CommandFrameContent frame,
            Action<TResponseFrame> callback, TimeSpan timeout) where TResponseFrame : CommandResponseFrameContent
        {
            frame.FrameId = GetNextFrameId();

            /* Make sure callback is in this context */
            SynchronizationContext context = SynchronizationContext.Current;
            var callbackProxy = new Action<CommandResponseFrameContent>(callbackFrame =>
            {
                if (context == null)
                    callback((TResponseFrame) callbackFrame);
                else context.Post(state => callback((TResponseFrame) callbackFrame), null);
            });

            ExecuteCallbacks.AddOrUpdate(frame.FrameId, b => callbackProxy, (b, source) => callbackProxy);

            await ExecuteAsync(frame);

            await Task.Delay(timeout);

            Action<CommandResponseFrameContent> action;
            ExecuteCallbacks.TryRemove(frame.FrameId, out action);
        }

        /// <summary>
        /// Get the controller sample source.
        /// </summary>
        public IObservable<SourcedSample> GetSampleSource()
        {
            return _sampleSource;
        }

        /// <summary>
        /// Get the controller received data source.
        /// </summary>
        public IObservable<SourcedData> GetReceivedDataSource()
        {
            return _receivedDataSource;
        }

        /// <summary>
        /// Start network discovery.  The discovery of a node will result in a <see cref="NodeDiscovered"/> event.
        /// </summary>
        public async Task DiscoverNetwork()
        {
            await DiscoverNetwork(NetworkDiscoveryTimeout);
        }

        /// <summary>
        /// Start network discovery.  The discovery of a node will result in a <see cref="NodeDiscovered"/> event.
        /// </summary>
        /// <param name="timeout">The amount of time to wait until discovery responses are ignored</param>
        /// <remarks>During network discovery nodes may be unresponsive</remarks>
        public async Task DiscoverNetwork(TimeSpan timeout)
        {
            var atCommandFrame = new AtCommandFrameContent(new NetworkDiscoveryCommand());

            await ExecuteMultiQueryAsync(atCommandFrame, new Action<AtCommandResponseFrame>(
                async frame =>
                {
                    var discoveryData = (NetworkDiscoveryResponseData) frame.Content.Data;

                    if (NodeDiscovered != null && discoveryData != null && !discoveryData.IsCoordinator)
                    {
                        var address = new NodeAddress(discoveryData.LongAddress, discoveryData.ShortAddress);

                        /* For some reason it doesn't like answering us during ND */
                        // TODO find better approach for this
                        XBeeNode node = null;
                        for (int i = 0; i < 5; i++)
                        {
                            try
                            {
                                node = await GetRemoteNodeAsync(address);
                                break;
                            }
                            catch (TimeoutException)
                            {
                            }
                            catch (AtCommandException)
                            {
                            }
                        }

                        if(node == null)
                            throw new TimeoutException();

                        var signalStrength = discoveryData.ReceivedSignalStrengthIndicator?.SignalStrength;

                        NodeDiscovered(this,
                            new NodeDiscoveredEventArgs(discoveryData.Name, signalStrength,
                                node));
                    }
                }), timeout);
        }

#if WINDOWS_UWP
        /// <summary>
        /// Try to find and open a local node.
        /// </summary>
        /// <returns>The controller or null if no controller was found</returns>
        public static async Task<XBeeController> FindAndOpen()
        {
            var controller = new XBeeController();

            string aqs = SerialDevice.GetDeviceSelector();
            var devices = await DeviceInformation.FindAllAsync(aqs);

            foreach (var device in devices)
            {
                try
                {
                    var serialDevice = await SerialDevice.FromIdAsync(device.Id);
                    await controller.OpenAsync(serialDevice);
                    return controller;
                }
                catch (InvalidOperationException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (ArgumentOutOfRangeException)
                {
                }
                catch (ArgumentException)
                {
                }
                catch (TimeoutException)
                {
                }
                catch (IOException)
                {
                }
            }

            return null;
        }
#else
        /// <summary>
        /// Try to find and open a local node.
        /// </summary>
        /// <param name="ports">Ports to scan</param>
        /// <param name="baudRate">Baud rate, typically 9600 or 115200</param>
        /// <returns>The controller or null if no controller was found</returns>
        public static async Task<XBeeController> FindAndOpen(IEnumerable<string> ports, int baudRate)
        {
            var controller = new XBeeController();

            foreach (var port in ports)
            {
                try
                {
                    await controller.OpenAsync(port, baudRate);
                    return controller;
                }
                catch (InvalidOperationException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (ArgumentOutOfRangeException)
                {
                }
                catch (ArgumentException)
                {
                }
                catch (TimeoutException)
                {
                }
                catch (IOException)
                {
                }
            }

            return null;
        }
#endif

        internal async Task Reset()
        {
            _modemResetTaskCompletionSource = new TaskCompletionSource<ModemStatus>();
            await ExecuteAtCommandAsync(new ResetCommand());

            var delayCancellationTokenSource = new CancellationTokenSource();
            Task delayTask = Task.Delay(ModemResetTimeout, delayCancellationTokenSource.Token);

            if (await Task.WhenAny(_modemResetTaskCompletionSource.Task, delayTask) == delayTask)
                throw new TimeoutException("No modem status received after reset.");

            delayCancellationTokenSource.Cancel();
            _modemResetTaskCompletionSource = null;
        }

        private XBeeNode CreateNode(HardwareVersion hardwareVersion, NodeAddress address = null)
        {
            switch (hardwareVersion)
            {
                case HardwareVersion.XBeeSeries1:
                    return new XBeeSeries1(this, HardwareVersion.XBeeSeries1, address);
                case HardwareVersion.XBeeProSeries1:
                    return new XBeeSeries1(this, HardwareVersion.XBeeProSeries1, address);
                case HardwareVersion.XBeeProS2:
                    return new XBeeSeries2(this, HardwareVersion.XBeeProS2, address);
                case HardwareVersion.XBeeProS2B:
                    return new XBeeSeries2(this, HardwareVersion.XBeeProS2B, address);
                case HardwareVersion.XBeePro900:
                    return new XBeePro900HP(this, HardwareVersion.XBeePro900, address);
                case HardwareVersion.XBeePro900HP:
                    return new XBeePro900HP(this, HardwareVersion.XBeePro900HP, address);
                default:
                    throw new NotSupportedException($"{hardwareVersion} not supported.");
            }
        }

        private void OnFrameReceived(object sender, FrameReceivedEventArgs e)
        {
            FrameContent content = e.FrameContent;

            if (content is ModemStatusFrame)
            {
                var modemStatusFrame = content as ModemStatusFrame;

                _modemResetTaskCompletionSource?.SetResult(modemStatusFrame.ModemStatus);
            }
            else if (content is CommandResponseFrameContent)
            {
                var commandResponse = content as CommandResponseFrameContent;

                byte frameId = commandResponse.FrameId;

                TaskCompletionSource<CommandResponseFrameContent> taskCompletionSource;
                if (ExecuteTaskCompletionSources.TryRemove(frameId, out taskCompletionSource))
                {
                    taskCompletionSource.SetResult(commandResponse);
                }
                else
                {
                    Action<CommandResponseFrameContent> callback;
                    if (ExecuteCallbacks.TryGetValue(frameId, out callback))
                    {
                        callback(commandResponse);
                    }
                }
            }
            else if (content is IRxIndicatorDataFrame)
            {
                var dataFrame = content as IRxIndicatorDataFrame;
                NodeAddress address = dataFrame.GetAddress();

                _receivedDataSource.Push(new SourcedData(address, dataFrame.Data));

                DataReceived?.Invoke(this, new SourcedDataReceivedEventArgs(address, dataFrame.Data));
            }
            else if (content is IRxIndicatorSampleFrame)
            {
                var sampleFrame = content as IRxIndicatorSampleFrame;
                NodeAddress address = sampleFrame.GetAddress();
                Sample sample = sampleFrame.GetSample();

                _sampleSource.Push(new SourcedSample(address, sample));

                SampleReceived?.Invoke(this,
                        new SourcedSampleReceivedEventArgs(address, sample.DigitalSampleState, sample.AnalogSamples));
            }
        }

        private byte GetNextFrameId()
        {
            lock (_frameIdLock)
            {
                unchecked
                {
                    _frameId++;
                }

                if (_frameId == 0)
                    _frameId = 1;

                return _frameId;
            }
        }

        public void Close()
        {
            if (IsOpen)
            {
                _connection.MemberSerializing -= OnMemberSerializing;
                _connection.MemberSerialized -= OnMemberSerialized;
                _connection.MemberDeserializing -= OnMemberDeserializing;
                _connection.MemberDeserialized -= OnMemberDeserialized;

                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }

        private void OnMemberSerialized(object sender, MemberSerializedEventArgs e)
        {
            FrameMemberSerialized?.Invoke(sender, e);
        }

        private void OnMemberDeserialized(object sender, MemberSerializedEventArgs e)
        {
            FrameMemberDeserialized?.Invoke(sender, e);
        }

        private void OnMemberSerializing(object sender, MemberSerializingEventArgs e)
        {
            FrameMemberSerializing?.Invoke(sender, e);
        }

        private void OnMemberDeserializing(object sender, MemberSerializingEventArgs e)
        {
            FrameMemberDeserializing?.Invoke(sender, e);
        }
    }
}