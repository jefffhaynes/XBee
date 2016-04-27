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

        private EventHandler<MemberSerializedEventArgs> _frameMemberSerialized;
        private EventHandler<MemberSerializedEventArgs> _frameMemberDeserialized;
        private EventHandler<MemberSerializingEventArgs> _frameMemberSerializing;
        private EventHandler<MemberSerializingEventArgs> _frameMemberDeserializing;

        // ReSharper disable DelegateSubtraction

        /// <summary>
        ///     Occurs after a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> FrameMemberSerialized
        {
            add
            {
                _frameMemberSerialized += value;
                var connection = _connection;
                if (connection != null)
                    connection.MemberSerialized += _frameMemberSerialized;
            }

            remove
            {
                _frameMemberSerialized -= value;
                var connection = _connection;
                if (connection != null)
                    connection.MemberSerialized -= _frameMemberSerialized;
            }
        }

        /// <summary>
        ///     Occurs after a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> FrameMemberDeserialized
        {
            add
            {
                _frameMemberDeserialized += value;
                var connection = _connection;
                if (connection != null)
                    connection.MemberDeserialized += _frameMemberDeserialized;
            }

            remove
            {
                _frameMemberDeserialized -= value;
                var connection = _connection;
                if (connection != null)
                    connection.MemberDeserialized -= _frameMemberDeserialized;
            }
        }

        /// <summary>
        ///     Occurs before a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> FrameMemberSerializing
        {
            add
            {
                _frameMemberSerializing += value;
                var connection = _connection;
                if (connection != null)
                    connection.MemberSerializing += _frameMemberSerializing;
            }

            remove
            {
                _frameMemberSerializing -= value;
                var connection = _connection;
                if (connection != null)
                    connection.MemberSerializing -= _frameMemberSerializing;
            }
        }

        /// <summary>
        ///     Occurs before a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> FrameMemberDeserializing
        {
            add
            {
                _frameMemberDeserializing += value;
                var connection = _connection;
                if (connection != null)
                    connection.MemberDeserializing += _frameMemberDeserializing;
            }

            remove
            {
                _frameMemberDeserializing -= value;
                var connection = _connection;
                if (connection != null)
                    connection.MemberDeserializing -= _frameMemberDeserializing;
            }
        }

        // ReSharper restore DelegateSubtraction

        public HardwareVersion? HardwareVersion { get; private set; }

        /// <summary>
        ///     Get the local node.
        /// </summary>
        public XBeeNode Local { get; private set; }

        public bool IsOpen => _connection?.IsOpen ?? false;

        [Obsolete("Use XBeeController(port, baudRate)")]
        public XBeeController()
        {
        }

        public XBeeController(string port, int baudRate)
        {
            Connect(port, baudRate);
        }

        public void Dispose()
        {
            Close();
            _sampleSource.Dispose();
            _receivedDataSource.Dispose();
        }

        /// <summary>
        ///     Occurs when a node is discovered during network discovery.
        /// </summary>
        public event EventHandler<NodeDiscoveredEventArgs> NodeDiscovered;

        /// <summary>
        ///     Occurs when data is received from a node.
        /// </summary>
        public event EventHandler<SourcedDataReceivedEventArgs> DataReceived;

        /// <summary>
        ///     Occurs when a sample is received from a node.
        /// </summary>
        public event EventHandler<SourcedSampleReceivedEventArgs> SampleReceived;

        /// <summary>
        ///     Occurs when a sensor sample is received from a node.
        /// </summary>
        public event EventHandler<SourcedSensorSampleReceivedEventArgs> SensorSampleReceived;

#if WINDOWS_UWP
    /// <summary>
    /// Open a local node.
    /// </summary>
    /// <param name="device">The serial device used to talk to the controller node</param>
    /// <returns></returns>
        public async Task OpenAsync(SerialDevice device)
#else
        /// <summary>
        ///     Open a local node.
        /// </summary>
        /// <param name="port">The COM port of the node</param>
        /// <param name="baudRate">The baud rate, typically 9600 or 115200 depending on the model</param>
        /// <returns></returns>
        [Obsolete("Use OpenAsync()")]
        public async Task OpenAsync(string port, int baudRate)
#endif
        {
            if (IsOpen)
                throw new InvalidOperationException(
                    "The controller is already connected, please close the existing connection.");

            _connection?.Dispose();

            if (_connection != null)
            {
                _connection.Open();
                return;
            }

#if WINDOWS_UWP
            _connection = new SerialConnection(device);
#else
            _connection = new SerialConnection(port, baudRate);
#endif

            try
            {
                Connect(port, baudRate);

                _connection.Open();

                await Initialize();
            }
            catch (Exception)
            {
                Close();
                throw;
            }
        }

        public async Task OpenAsync()
        {
            _connection.Open();
            await Initialize();
            await Task.FromResult(0);
        }

        private void Connect(string port, int baudRate)
        {
            _connection = new SerialConnection(port, baudRate);

            if (_frameMemberDeserialized != null)
                _connection.MemberDeserialized += _frameMemberDeserialized;

            if (_frameMemberDeserializing != null)
                _connection.MemberDeserializing += _frameMemberDeserializing;

            if (_frameMemberSerialized != null)
                _connection.MemberSerialized += _frameMemberSerialized;

            if (_frameMemberDeserialized != null)
                _connection.MemberDeserialized += _frameMemberDeserialized;

            _connection.FrameReceived += OnFrameReceived;
        }

        private readonly SemaphoreSlim _initializeSemaphoreSlim = new SemaphoreSlim(1);
        private bool _isInitialized;

        private async Task Initialize()
        {
            if (_isInitialized)
                return;

            await _initializeSemaphoreSlim.WaitAsync();

            if (_isInitialized)
                return;

            try
            {
                /* Unfortunately the protocol changes based on what type of hardware we're using... */
                HardwareVersion = await GetHardwareVersion();
                _connection.CoordinatorHardwareVersion = HardwareVersion;

                /* We want the receiver to have the hardware version in context so cycle the connection */
                _connection.Close();

                // This is to address a .net (windows?) serial port management issue
                await TaskExtensions.Retry(() => _connection.Open(),
                    TimeSpan.FromSeconds(3), typeof (UnauthorizedAccessException));

                // ReSharper disable PossibleInvalidOperationException
                Local = CreateNode(HardwareVersion.Value);
                // ReSharper restore PossibleInvalidOperationException

                _isInitialized = true;
            }
            finally
            {
                _initializeSemaphoreSlim.Release();
            }
        }

        /// <summary>
        ///     Open a remote node.
        /// </summary>
        /// <param name="address">The address of the remote node</param>
        /// <returns>The remote node</returns>
        [Obsolete("Use GetNodeAsync")]
        public async Task<XBeeNode> GetRemoteAsync(NodeAddress address)
        {
            return await GetNodeAsync(address);
        }

        /// <summary>
        ///     Open a remote node.
        /// </summary>
        /// <param name="address">The address of the remote node</param>
        /// <returns>The remote node</returns>
        [Obsolete("Use GetNodeAsync")]
        public async Task<XBeeNode> GetRemoteNodeAsync(NodeAddress address)
        {
            return await GetNodeAsync(address);
        }

        /// <summary>
        /// Create a node.
        /// </summary>
        /// <param name="address">The address of the node or null for the controller node.</param>
        /// <param name="autodetectHardwareVersion">If true query node for hardware version.  Otherwise assume controller version.</param>
        /// <returns>The specified node.</returns>
        public async Task<XBeeNode> GetNodeAsync(NodeAddress address = null, bool autodetectHardwareVersion = true)
        {
            await Initialize();

            if (address == null)
                return Local;

            HardwareVersion version;

            if (autodetectHardwareVersion)
            {
                if (!IsOpen)
                    throw new InvalidOperationException("Connection closed.");

                version = await
                    TaskExtensions.Retry(async () => await GetHardwareVersion(address), TimeSpan.FromSeconds(5),
                        typeof(TimeoutException), typeof(AtCommandException));
            }
            else
            {
                version = Local.HardwareVersion;
            }

            return await Task.FromResult(CreateNode(version, address));
        }

        /// <summary>
        /// Create a node.
        /// </summary>
        /// <param name="address">The address of the node or null for the controller node.</param>
        /// <param name="version">The hardware version to use for the specified node.</param>
        /// <returns>The specified node.</returns>
        public async Task<XBeeNode> GetNodeAsync(NodeAddress address, HardwareVersion version)
        {
            return await Task.FromResult(CreateNode(version, address));
        }

        private async Task<HardwareVersion> GetHardwareVersion(NodeAddress address = null)
        {
            var version =
                await
                    ExecuteAtQueryAsync<HardwareVersionResponseData>(new HardwareVersionCommand(), address,
                        TimeSpan.FromSeconds(1));

            return version.HardwareVersion;
        }

        /// <summary>
        ///     Send a frame to this node.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        internal async Task ExecuteAsync(FrameContent frame)
        {
            await ExecuteAsync(frame, CancellationToken.None);
        }

        /// <summary>
        ///     Send a frame to this node.
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal async Task ExecuteAsync(FrameContent frame, CancellationToken cancellationToken)
        {
            if (!IsOpen)
                throw new InvalidOperationException("Controller must be open to execute commands.");

            await _connection.Send(frame, cancellationToken);
        }

        /// <summary>
        ///     Send an AT command to this node.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        internal async Task ExecuteAtCommand(AtCommand command, NodeAddress address = null)
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
        ///     Send a frame to this node and wait for a response.
        /// </summary>
        /// <typeparam name="TResponseFrame">The expected response type</typeparam>
        /// <param name="frame">The frame to send</param>
        /// <param name="timeout">Timeout</param>
        /// <returns>The response frame</returns>
        internal async Task<TResponseFrame> ExecuteQueryAsync<TResponseFrame>(CommandFrameContent frame, TimeSpan timeout)
            where TResponseFrame : CommandResponseFrameContent
        {
            return await ExecuteQueryAsync<TResponseFrame>(frame, timeout, CancellationToken.None);
        }

        /// <summary>
        ///     Send a frame to this node and wait for a response.
        /// </summary>
        /// <typeparam name="TResponseFrame">The expected response type</typeparam>
        /// <param name="frame">The frame to send</param>
        /// <param name="timeout">Timeout</param>
        /// <param name="cancellationToken">A cancellation token used to cancel the query.</param>
        /// <returns>The response frame</returns>
        internal async Task<TResponseFrame> ExecuteQueryAsync<TResponseFrame>(CommandFrameContent frame, TimeSpan timeout,
            CancellationToken cancellationToken)
            where TResponseFrame : CommandResponseFrameContent
        {
            frame.FrameId = GetNextFrameId();

            var delayCancellationTokenSource = new CancellationTokenSource();
            var delayTask = Task.Delay(timeout, delayCancellationTokenSource.Token);

            var taskCompletionSource =
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
        ///     Send a frame to this node and wait for a response using a default timeout.
        /// </summary>
        /// <typeparam name="TResponseFrame">The expected response type</typeparam>
        /// <param name="frame">The frame to send</param>
        /// <returns>The response frame</returns>
        internal Task<TResponseFrame> ExecuteQueryAsync<TResponseFrame>(CommandFrameContent frame)
            where TResponseFrame : CommandResponseFrameContent
        {
            return ExecuteQueryAsync<TResponseFrame>(frame, CancellationToken.None);
        }

        /// <summary>
        ///     Send a frame to this node and wait for a response using a default timeout.
        /// </summary>
        /// <typeparam name="TResponseFrame">The expected response type</typeparam>
        /// <param name="frame">The frame to send</param>
        /// <param name="cancellationToken">Used to cancel the operation</param>
        /// <returns>The response frame</returns>
        internal Task<TResponseFrame> ExecuteQueryAsync<TResponseFrame>(CommandFrameContent frame,
            CancellationToken cancellationToken)
            where TResponseFrame : CommandResponseFrameContent
        {
            return ExecuteQueryAsync<TResponseFrame>(frame, DefaultRemoteQueryTimeout, cancellationToken);
        }

        /// <summary>
        ///     Send an AT command to a node and wait for a response.
        /// </summary>
        /// <typeparam name="TResponseData">The expected response data type</typeparam>
        /// <param name="command">The command to send</param>
        /// <param name="address">The address of the node.  If this is null the command will be sent to the local node.</param>
        /// <returns>The response data</returns>
        internal async Task<TResponseData> ExecuteAtQueryAsync<TResponseData>(AtCommand command,
            NodeAddress address = null)
            where TResponseData : AtCommandResponseFrameData
        {
            var timeout = address == null ? DefaultLocalQueryTimeout : DefaultRemoteQueryTimeout;
            return await ExecuteAtQueryAsync<TResponseData>(command, address, timeout);
        }

        /// <summary>
        ///     Send an AT command to a node and wait for a response.
        /// </summary>
        /// <typeparam name="TResponseData">The expected response data type</typeparam>
        /// <param name="command">The command to send</param>
        /// <param name="address">The address of the node.  If this is null the command will be sent to the local node.</param>
        /// <param name="timeout"></param>
        /// <returns>The response data</returns>
        internal async Task<TResponseData> ExecuteAtQueryAsync<TResponseData>(AtCommand command,
            NodeAddress address, TimeSpan timeout)
            where TResponseData : AtCommandResponseFrameData
        {
            AtCommandResponseFrameContent responseContent;

            if (address == null)
            {
                var atCommandFrame = new AtCommandFrameContent(command);
                var response = await ExecuteQueryAsync<AtCommandResponseFrame>(atCommandFrame, timeout);
                responseContent = response.Content;
            }
            else
            {
                address.ShortAddress = address.LongAddress.IsBroadcast ? ShortAddress.Broadcast : ShortAddress.Disabled;

                var remoteCommand = new RemoteAtCommandFrameContent(address, command);
                var response =
                    await ExecuteQueryAsync<RemoteAtCommandResponseFrame>(remoteCommand, timeout);
                responseContent = response.Content;
            }

            if (responseContent.Status != AtCommandStatus.Success)
                throw new AtCommandException(responseContent.Status);

            return responseContent.Data as TResponseData;
        }

        /// <summary>
        ///     Execute an AT command on a node without waiting for a response.
        /// </summary>
        /// <param name="command">The AT command to execute</param>
        /// <param name="address">The address of the node.  If this is null the command will be execute on the local node.</param>
        /// <returns></returns>
        internal async Task ExecuteAtCommandAsync(AtCommand command, NodeAddress address = null)
        {
            await ExecuteAtQueryAsync<AtCommandResponseFrameData>(command, address);
        }

        /// <summary>
        ///     Execute a command and wait for responses from multiple nodes.
        /// </summary>
        /// <typeparam name="TResponseFrame">The expected response type</typeparam>
        /// <param name="frame">The frame to send.</param>
        /// <param name="callback">This will be called when a response is received within the timeout period.</param>
        /// <param name="timeout">The amount of time to wait before responses will be ignored</param>
        private async Task ExecuteMultiQueryAsync<TResponseFrame>(CommandFrameContent frame,
            Action<TResponseFrame> callback, TimeSpan timeout) where TResponseFrame : CommandResponseFrameContent
        {
            frame.FrameId = GetNextFrameId();

            /* Make sure callback is in this context */
            var context = SynchronizationContext.Current;
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
        ///     Get the controller sample source.
        /// </summary>
        public IObservable<SourcedSample> GetSampleSource()
        {
            return _sampleSource;
        }

        /// <summary>
        ///     Get the controller received data source.
        /// </summary>
        public IObservable<SourcedData> GetReceivedDataSource()
        {
            return _receivedDataSource;
        }

        /// <summary>
        ///     Start network discovery.  The discovery of a node will result in a <see cref="NodeDiscovered" /> event.
        /// </summary>
        public async Task DiscoverNetworkAsync()
        {
            await DiscoverNetworkAsync(NetworkDiscoveryTimeout);
        }

        /// <summary>
        ///     Start network discovery.  The discovery of a node will result in a <see cref="NodeDiscovered" /> event.
        /// </summary>
        /// <param name="timeout">The amount of time to wait until discovery responses are ignored</param>
        /// <remarks>During network discovery nodes may be unresponsive</remarks>
        public async Task DiscoverNetworkAsync(TimeSpan timeout)
        {
            var atCommandFrame = new AtCommandFrameContent(new NetworkDiscoveryCommand());

            await ExecuteMultiQueryAsync(atCommandFrame, new Action<AtCommandResponseFrame>(
                async frame =>
                {
                    var discoveryData = (NetworkDiscoveryResponseData) frame.Content.Data;

                    if (NodeDiscovered != null && discoveryData != null && !discoveryData.IsCoordinator)
                    {
                        var address = new NodeAddress(discoveryData.LongAddress, discoveryData.ShortAddress);

                        // XBees have trouble recovering from discovery
                        await Task.Delay(500);

                        try
                        {
                            var node = await GetNodeAsync(address);

                            var signalStrength = discoveryData.ReceivedSignalStrengthIndicator?.SignalStrength;

                            NodeDiscovered(this,
                                new NodeDiscoveredEventArgs(discoveryData.Name, signalStrength,
                                    node));
                        }
                        catch (TimeoutException)
                        {
                            /* if we timeout getting the remote node info, no need to bubble up.  
                             * We just won't include the node in discovery */
                        }
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
        ///     Try to find and open a local node.
        /// </summary>
        /// <param name="ports">Ports to scan</param>
        /// <param name="baudRate">Baud rate, typically 9600 or 115200</param>
        /// <returns>The controller or null if no controller was found</returns>
        public static async Task<XBeeController> FindAndOpenAsync(IEnumerable<string> ports, int baudRate)
        {
            foreach (var port in ports)
            {
                try
                {
                    var controller = new XBeeController(port, baudRate);
                    await controller.OpenAsync();
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

        /// <summary>
        ///     Try to find and open a local node.
        /// </summary>
        /// <param name="ports">Ports to scan</param>
        /// <param name="baudRate">Baud rate, typically 9600 or 115200</param>
        /// <returns>The controller or null if no controller was found</returns>
        public static async Task<XBeeController> FindAsync(IEnumerable<string> ports, int baudRate)
        {
            var controller = await FindAndOpenAsync(ports, baudRate);
            controller.Close();
            return controller;
        }

        internal async Task Reset()
        {
            _modemResetTaskCompletionSource = new TaskCompletionSource<ModemStatus>();
            await ExecuteAtCommandAsync(new ResetCommand());

            var delayCancellationTokenSource = new CancellationTokenSource();
            var delayTask = Task.Delay(ModemResetTimeout, delayCancellationTokenSource.Token);

            if (await Task.WhenAny(_modemResetTaskCompletionSource.Task, delayTask) == delayTask)
                throw new TimeoutException("No modem status received after reset.");

            delayCancellationTokenSource.Cancel();
            _modemResetTaskCompletionSource = null;
        }

        private XBeeNode CreateNode(HardwareVersion hardwareVersion, NodeAddress address = null)
        {
            switch (hardwareVersion)
            {
                case Frames.AtCommands.HardwareVersion.XBeeSeries1:
                    return new XBeeSeries1(this, Frames.AtCommands.HardwareVersion.XBeeSeries1, address);
                case Frames.AtCommands.HardwareVersion.XBeeProSeries1:
                    return new XBeeSeries1(this, Frames.AtCommands.HardwareVersion.XBeeProSeries1, address);
                case Frames.AtCommands.HardwareVersion.ZNetZigBeeS2:
                    return new XBeeSeries2(this, Frames.AtCommands.HardwareVersion.ZNetZigBeeS2, address);
                case Frames.AtCommands.HardwareVersion.XBeeProS2:
                    return new XBeeSeries2(this, Frames.AtCommands.HardwareVersion.XBeeProS2, address);
                case Frames.AtCommands.HardwareVersion.XBeeProS2B:
                    return new XBeeSeries2(this, Frames.AtCommands.HardwareVersion.XBeeProS2B, address);
                case Frames.AtCommands.HardwareVersion.XBee24C:
                    return new XBeeSeries2(this, Frames.AtCommands.HardwareVersion.XBee24C, address);
                case Frames.AtCommands.HardwareVersion.XBeePro900:
                    return new XBeePro900HP(this, Frames.AtCommands.HardwareVersion.XBeePro900, address);
                case Frames.AtCommands.HardwareVersion.XBeePro900HP:
                    return new XBeePro900HP(this, Frames.AtCommands.HardwareVersion.XBeePro900HP, address);
                default:
                    throw new NotSupportedException($"{hardwareVersion} not supported.");
            }
        }

        private void OnFrameReceived(object sender, FrameReceivedEventArgs e)
        {
            var content = e.FrameContent;

            if (content is ModemStatusFrame)
            {
                var modemStatusFrame = content as ModemStatusFrame;

                _modemResetTaskCompletionSource?.SetResult(modemStatusFrame.ModemStatus);
            }
            else if (content is CommandResponseFrameContent)
            {
                var commandResponse = content as CommandResponseFrameContent;

                var frameId = commandResponse.FrameId;

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
                var address = dataFrame.GetAddress();

                _receivedDataSource.Push(new SourcedData(address, dataFrame.Data));

                DataReceived?.Invoke(this, new SourcedDataReceivedEventArgs(address, dataFrame.Data));
            }
            else if (content is IRxIndicatorSampleFrame)
            {
                var sampleFrame = content as IRxIndicatorSampleFrame;
                var address = sampleFrame.GetAddress();
                var sample = sampleFrame.GetSample();

                _sampleSource.Push(new SourcedSample(address, sample));

                SampleReceived?.Invoke(this,
                    new SourcedSampleReceivedEventArgs(address, sample.DigitalChannels, sample.DigitalSampleState,
                        sample.AnalogChannels, sample.AnalogSamples));
            }
            else if (content is SensorReadIndicatorFrame)
            {
                var sensorFrame = content as SensorReadIndicatorFrame;
                var sensorSample = new SensorSample(sensorFrame.OneWireSensor, 
                    sensorFrame.SensorValueA, 
                    sensorFrame.SensorValueB, 
                    sensorFrame.SensorValueC, 
                    sensorFrame.SensorValueD, 
                    sensorFrame.TemperatureCelsius);

                var address = sensorFrame.GetAddress();

                SensorSampleReceived?.Invoke(this,
                    new SourcedSensorSampleReceivedEventArgs(address, sensorSample.OneWireSensor,
                        sensorSample.SensorValueA, sensorSample.SensorValueB, sensorSample.SensorValueC,
                        sensorSample.SensorValueD, sensorSample.TemperatureCelsius));
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
                _connection.Close();
        }

        #region Deprecated

        [Obsolete("Use DiscoverNetworkAsync")]
        public async Task DiscoverNetwork()
        {
            await DiscoverNetworkAsync();
        }

        [Obsolete("Use DiscoverNetworkAsync")]
        public async Task DiscoverNetwork(TimeSpan timeout)
        {
            await DiscoverNetworkAsync(timeout);
        }

        [Obsolete("Use FindAsync")]
        public static async Task<XBeeController> Find(IEnumerable<string> ports, int baudRate)
        {
            return await FindAsync(ports, baudRate);
        }

        [Obsolete("Use FindAndOpenAsync")]
        public static async Task<XBeeController> FindAndOpen(IEnumerable<string> ports, int baudRate)
        {
            return await FindAndOpenAsync(ports, baudRate);
        }

        #endregion
    }
}