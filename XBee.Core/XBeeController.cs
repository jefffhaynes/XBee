using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BinarySerialization;
using XBee.Devices;
using XBee.Frames;
using XBee.Frames.AtCommands;
using XBee.Observable;

namespace XBee
{
    public class XBeeController : IDisposable
    {
        private static readonly ConcurrentDictionary<byte, TaskCompletionSource<CommandResponseFrameContent>>
            ExecuteTaskCompletionSources =
                new ConcurrentDictionary<byte, TaskCompletionSource<CommandResponseFrameContent>>();

        private static readonly ConcurrentDictionary<byte, Action<CommandResponseFrameContent>> ExecuteCallbacks =
            new ConcurrentDictionary<byte, Action<CommandResponseFrameContent>>();

        private static readonly TimeSpan ModemResetTimeout = TimeSpan.FromSeconds(300);
        private static readonly TimeSpan DefaultRemoteQueryTimeout = TimeSpan.FromSeconds(300);
        private static readonly TimeSpan DefaultLocalQueryTimeout = TimeSpan.FromSeconds(300);

        private static readonly TimeSpan NetworkDiscoveryTimeout = TimeSpan.FromSeconds(30);
        private readonly object _frameIdLock = new object();

        private readonly SemaphoreSlim _initializeSemaphoreSlim = new SemaphoreSlim(1);

        private readonly Source<SourcedData> _receivedDataSource = new Source<SourcedData>();
        private readonly Source<SourcedSample> _sampleSource = new Source<SourcedSample>();

        //private SerialConnection _connection;
        private byte _frameId = byte.MinValue;
        private bool _isInitialized;
        private TaskCompletionSource<ModemStatus> _modemResetTaskCompletionSource;

        //public XBeeController(string port, int baudRate)
        //{
        //    Connect(port, baudRate);
        //}

        private readonly ISerialDevice _serialDevice;

        public XBeeController(ISerialDevice serialDevice)
        {
            _serialDevice = serialDevice;
        }

        public HardwareVersion? HardwareVersion { get; private set; }

        /// <summary>
        ///     Get the local node.
        /// </summary>
        public XBeeNode Local { get; private set; }

        //public bool IsOpen => _connection?.IsOpen ?? false;

        public void Dispose()
        {
            //Close();
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

        /// <summary>
        ///     Occurs when an SMS message is received.
        /// </summary>
        public event EventHandler<SmsReceivedEventArgs> SmsReceived;

        /// <summary>
        ///     Occurs when IP data is received.
        /// </summary>
        public event EventHandler<InternetDataReceivedEventArgs> InternetDataReceived;

        /// <summary>
        /// Occurs when a node identification is received.
        /// </summary>
        public event EventHandler<NodeIdentificationEventArgs> NodeIdentificationReceived;

        /// <summary>
        ///     Create a node.
        /// </summary>
        /// <param name="address">The address of the node or null for the controller node.</param>
        /// <param name="autodetectHardwareVersion">If true query node for hardware version.  Otherwise assume controller version.</param>
        /// <returns>The specified node.</returns>
        public async Task<XBeeNode> GetNodeAsync(NodeAddress address = null, bool autodetectHardwareVersion = true)
        {
            await Initialize().ConfigureAwait(false);

            if (address == null)
            {
                return Local;
            }

            HardwareVersion version;

            if (autodetectHardwareVersion)
            {
                version = await
                    TaskExtensions.Retry(async () => await GetHardwareVersion(address), TimeSpan.FromSeconds(5),
                        typeof(TimeoutException), typeof(AtCommandException)).ConfigureAwait(false);
            }
            else
            {
                version = Local.HardwareVersion;
            }

            return await Task.FromResult(CreateNode(version, address)).ConfigureAwait(false);
        }

        /// <summary>
        ///     Create a node.
        /// </summary>
        /// <param name="address">The address of the node or null for the controller node.</param>
        /// <param name="version">The hardware version to use for the specified node.</param>
        /// <returns>The specified node.</returns>
        public Task<XBeeNode> GetNodeAsync(NodeAddress address, HardwareVersion version)
        {
            return Task.FromResult(CreateNode(version, address));
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
        public Task DiscoverNetworkAsync()
        {
            return DiscoverNetworkAsync(NetworkDiscoveryTimeout);
        }

        /// <summary>
        ///     Start network discovery.  The discovery of a node will result in a <see cref="NodeDiscovered" /> event.
        /// </summary>
        /// <param name="timeout">The amount of time to wait until discovery responses are ignored</param>
        /// <param name="cancellationToken"></param>
        /// <remarks>During network discovery nodes may be unresponsive</remarks>
        public Task DiscoverNetworkAsync(TimeSpan timeout, CancellationToken cancellationToken = default(CancellationToken))
        {
            var atCommandFrame = new AtCommandFrameContent(new NetworkDiscoveryCommand());

            return ExecuteMultiQueryAsync(atCommandFrame, new Action<AtCommandResponseFrame>(
                async frame =>
                {
                    var discoveryData = (NetworkDiscoveryResponseData) frame.Content.Data;

                    if (NodeDiscovered == null || discoveryData?.LongAddress == null || discoveryData.IsCoordinator)
                    {
                        return;
                    }

                    var address = new NodeAddress(discoveryData.LongAddress, discoveryData.ShortAddress);

                    // XBees have trouble recovering from discovery
                    await Task.Delay(500, cancellationToken);

                    try
                    {
                        var node = await GetNodeAsync(address);

                        var signalStrength = discoveryData.ReceivedSignalStrengthIndicator?.SignalStrength;

                        NodeDiscovered?.Invoke(this,
                            new NodeDiscoveredEventArgs(discoveryData.Name, signalStrength,
                                node));
                    }
                    catch (TimeoutException)
                    {
                        /* if we timeout getting the remote node info, no need to bubble up.  
                             * We just won't include the node in discovery */
                    }
                }), timeout, cancellationToken);
        }

        /// <summary>
        ///     Try to find and open a local node.
        /// </summary>
        /// <param name="ports">Ports to scan</param>
        /// <param name="baudRate">Baud rate, typically 9600 or 115200</param>
        /// <returns>The controller or null if no controller was found</returns>
        //public static async Task<XBeeController> FindAndOpenAsync(IEnumerable<string> ports, int baudRate)
        //{
        //    foreach (var port in ports)
        //    {
        //        try
        //        {
        //            var controller = new XBeeController(port, baudRate);
        //            await controller.OpenAsync().ConfigureAwait(false);
        //            return controller;
        //        }
        //        catch (InvalidOperationException)
        //        {
        //        }
        //        catch (UnauthorizedAccessException)
        //        {
        //        }
        //        catch (ArgumentOutOfRangeException)
        //        {
        //        }
        //        catch (ArgumentException)
        //        {
        //        }
        //        catch (TimeoutException)
        //        {
        //        }
        //        catch (IOException)
        //        {
        //        }
        //    }

        //    return null;
        //}
        

        /// <summary>
        ///     Send an AT command to this node.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="address"></param>
        /// <param name="queueLocal"></param>
        /// <returns></returns>
        internal void ExecuteAtCommand(AtCommand command, NodeAddress address = null, bool queueLocal = false)
        {
            if (address == null)
            {
                var atCommandFrame = queueLocal
                    ? new AtQueuedCommandFrameContent(command)
                    : new AtCommandFrameContent(command);

                Execute(atCommandFrame);
            }

            var remoteCommand = new RemoteAtCommandFrameContent(address, command);
            Execute(remoteCommand);
        }

        /// <summary>
        ///     Send a frame to this node and wait for a response.
        /// </summary>
        /// <typeparam name="TResponseFrame">The expected response type</typeparam>
        /// <param name="frame">The frame to send</param>
        /// <param name="timeout">Timeout</param>
        /// <returns>The response frame</returns>
        internal Task<TResponseFrame> ExecuteQueryAsync<TResponseFrame>(CommandFrameContent frame,
            TimeSpan timeout)
            where TResponseFrame : CommandResponseFrameContent
        {
            return ExecuteQueryAsync<TResponseFrame>(frame, timeout, CancellationToken.None);
        }

        /// <summary>
        ///     Send a frame to this node and wait for a response.
        /// </summary>
        /// <typeparam name="TResponseFrame">The expected response type</typeparam>
        /// <param name="frame">The frame to send</param>
        /// <param name="timeout">Timeout</param>
        /// <param name="cancellationToken">A cancellation token used to cancel the query.</param>
        /// <returns>The response frame</returns>
        internal async Task<TResponseFrame> ExecuteQueryAsync<TResponseFrame>(CommandFrameContent frame,
            TimeSpan timeout,
            CancellationToken cancellationToken)
            where TResponseFrame : CommandResponseFrameContent
        {
            frame.FrameId = GetNextFrameId();
            
            var delayTask = Task.Delay(timeout, cancellationToken);
            
            var taskCompletionSource =
                ExecuteTaskCompletionSources.AddOrUpdate(frame.FrameId,
                    b => new TaskCompletionSource<CommandResponseFrameContent>(),
                    (b, source) => new TaskCompletionSource<CommandResponseFrameContent>());

            Execute(frame);

            if (await Task.WhenAny(taskCompletionSource.Task, delayTask).ConfigureAwait(false) !=
                taskCompletionSource.Task)
            {
                throw new TimeoutException();
            }
            
            return await taskCompletionSource.Task.ConfigureAwait(false) as TResponseFrame;
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
        /// <param name="queueLocal">Queue this command for deferred execution if issued to a local controller.</param>
        /// <returns>The response data</returns>
        internal Task<TResponseData> ExecuteAtQueryAsync<TResponseData>(AtCommand command,
            NodeAddress address = null, bool queueLocal = false)
            where TResponseData : AtCommandResponseFrameData
        {
            var timeout = address == null ? DefaultLocalQueryTimeout : DefaultRemoteQueryTimeout;
            return ExecuteAtQueryAsync<TResponseData>(command, address, timeout, queueLocal);
        }

        /// <summary>
        ///     Send an AT command to a node and wait for a response.
        /// </summary>
        /// <typeparam name="TResponseData">The expected response data type</typeparam>
        /// <param name="command">The command to send</param>
        /// <param name="address">The address of the node.  If this is null the command will be sent to the local node.</param>
        /// <param name="timeout"></param>
        /// <param name="queueLocal">Queue this command for deferred execution if issued to a local controller.</param>
        /// <returns>The response data</returns>
        internal async Task<TResponseData> ExecuteAtQueryAsync<TResponseData>(AtCommand command,
            NodeAddress address, TimeSpan timeout, bool queueLocal = false)
            where TResponseData : AtCommandResponseFrameData
        {
            AtCommandResponseFrameContent responseContent;

            if (address == null)
            {
                var atCommandFrame = queueLocal
                    ? new AtQueuedCommandFrameContent(command)
                    : new AtCommandFrameContent(command);
                var response = await ExecuteQueryAsync<AtCommandResponseFrame>(atCommandFrame, timeout)
                    .ConfigureAwait(false);
                responseContent = response.Content;
            }
            else
            {
                address.ShortAddress = address.LongAddress.IsBroadcast ? ShortAddress.Broadcast : ShortAddress.Disabled;

                var remoteCommand = new RemoteAtCommandFrameContent(address, command);
                var response = await ExecuteQueryAsync<RemoteAtCommandResponseFrame>(remoteCommand, timeout)
                    .ConfigureAwait(false);
                responseContent = response.Content;
            }

            if (responseContent.Status != AtCommandStatus.Success)
            {
                throw new AtCommandException(responseContent.Status);
            }

            return responseContent.Data as TResponseData;
        }

        /// <summary>
        ///     Execute an AT command on a node without waiting for a response.
        /// </summary>
        /// <param name="command">The AT command to execute</param>
        /// <param name="address">The address of the node.  If this is null the command will be execute on the local node.</param>
        /// <param name="queueLocal">Queue this command for deferred execution if issued to a local controller.</param>
        /// <returns></returns>
        internal Task ExecuteAtCommandAsync(AtCommand command, NodeAddress address = null,
            bool queueLocal = false)
        {
            return ExecuteAtQueryAsync<AtCommandResponseFrameData>(command, address, queueLocal);
        }

        internal async Task Reset()
        {
            _modemResetTaskCompletionSource = new TaskCompletionSource<ModemStatus>();
            await ExecuteAtCommandAsync(new ResetCommand()).ConfigureAwait(false);

            var delayCancellationTokenSource = new CancellationTokenSource();
            var delayTask = Task.Delay(ModemResetTimeout, delayCancellationTokenSource.Token);

            if (await Task.WhenAny(_modemResetTaskCompletionSource.Task, delayTask).ConfigureAwait(false) == delayTask)
            {
                throw new TimeoutException("No modem status received after reset.");
            }

            delayCancellationTokenSource.Cancel();
            _modemResetTaskCompletionSource = null;
        }

        //private void Connect(string port, int baudRate)
        //{
        //    _connection = new SerialConnection(port, baudRate);

        //    if (_frameMemberDeserialized != null)
        //    {
        //        _connection.MemberDeserialized += _frameMemberDeserialized;
        //    }

        //    if (_frameMemberDeserializing != null)
        //    {
        //        _connection.MemberDeserializing += _frameMemberDeserializing;
        //    }

        //    if (_frameMemberSerialized != null)
        //    {
        //        _connection.MemberSerialized += _frameMemberSerialized;
        //    }

        //    if (_frameMemberDeserialized != null)
        //    {
        //        _connection.MemberDeserialized += _frameMemberDeserialized;
        //    }

        //    _connection.FrameReceived += OnFrameReceived;
        //}

        private async Task Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            await _initializeSemaphoreSlim.WaitAsync().ConfigureAwait(false);

            if (_isInitialized)
            {
                return;
            }

            try
            {
                /* Unfortunately the protocol changes based on what type of hardware we're using... */
                HardwareVersion = await GetHardwareVersion().ConfigureAwait(false);
                _frameContext.ControllerHardwareVersion = HardwareVersion;

                // start receiving frames
                Listen();

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

        private async Task<HardwareVersion> GetHardwareVersion(NodeAddress address = null)
        {
            var version =
                await
                    ExecuteAtQueryAsync<HardwareVersionResponseData>(new HardwareVersionCommand(), address,
                        TimeSpan.FromSeconds(1)).ConfigureAwait(false);

            return version.HardwareVersion;
        }

        /// <summary>
        ///     Execute a command and wait for responses from multiple nodes.
        /// </summary>
        /// <typeparam name="TResponseFrame">The expected response type</typeparam>
        /// <param name="frame">The frame to send.</param>
        /// <param name="callback">This will be called when a response is received within the timeout period.</param>
        /// <param name="timeout">The amount of time to wait before responses will be ignored</param>
        /// <param name="cancellationToken"></param>
        private async Task ExecuteMultiQueryAsync<TResponseFrame>(CommandFrameContent frame,
            Action<TResponseFrame> callback, TimeSpan timeout, CancellationToken cancellationToken) where TResponseFrame : CommandResponseFrameContent
        {
            frame.FrameId = GetNextFrameId();

            /* Make sure callback is in this context */
            var context = SynchronizationContext.Current;
            var callbackProxy = new Action<CommandResponseFrameContent>(callbackFrame =>
            {
                if (context == null)
                {
                    callback((TResponseFrame) callbackFrame);
                }
                else
                {
                    context.Post(state => callback((TResponseFrame) callbackFrame), null);
                }
            });

            ExecuteCallbacks.AddOrUpdate(frame.FrameId, b => callbackProxy, (b, source) => callbackProxy);

            Execute(frame);

            await Task.Delay(timeout, cancellationToken);

            Action<CommandResponseFrameContent> action;
            ExecuteCallbacks.TryRemove(frame.FrameId, out action);
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
                case Frames.AtCommands.HardwareVersion.XBeeProSX:
                    return new XBeePro900HP(this, Frames.AtCommands.HardwareVersion.XBeeProSX, address);
                case Frames.AtCommands.HardwareVersion.XBeeCellular:
                    return new XBeeCellular(this, Frames.AtCommands.HardwareVersion.XBeeCellular, address);
                default:
                    throw new NotSupportedException($"{hardwareVersion} not supported.");
            }
        }

        private readonly BinarySerializer _serializer = new BinarySerializer {Endianness = Endianness.Big};
        private CancellationTokenSource _listenerCancellationTokenSource;
        private readonly FrameContext _frameContext = new FrameContext(null);

        private void Listen()
        {
            _listenerCancellationTokenSource = new CancellationTokenSource();

            Task.Run(async () =>
            {
                var cancellationToken = _listenerCancellationTokenSource.Token;

                while (!_listenerCancellationTokenSource.IsCancellationRequested)
                {
                    var stream = new SerialDeviceStream(_serialDevice);
                    var frame = await _serializer.DeserializeAsync<Frame>(stream, _frameContext, cancellationToken)
                        .ConfigureAwait(false);

                    ProcessFrame(frame.Payload.Content);
                }
            });
        }

        private void Execute(FrameContent frameContent)
        {
            var stream = new SerialDeviceStream(_serialDevice);
            var frame = new Frame(frameContent);
            _serializer.Serialize(stream, frame);
        }

        private void ProcessFrame(FrameContent content)
        {
            if (content is ModemStatusFrame modemStatusFrame)
            {
                _modemResetTaskCompletionSource?.SetResult(modemStatusFrame.ModemStatus);
            }
            else if (content is CommandResponseFrameContent commandResponse)
            {
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
            else if (content is IRxIndicatorDataFrame dataFrame)
            {
                var address = dataFrame.GetAddress();

                _receivedDataSource.Push(new SourcedData(address, dataFrame.Data));

                DataReceived?.Invoke(this, new SourcedDataReceivedEventArgs(address, dataFrame.Data));
            }
            else if (content is IRxIndicatorSampleFrame sampleFrame)
            {
                var address = sampleFrame.GetAddress();
                var sample = sampleFrame.GetSample();

                _sampleSource.Push(new SourcedSample(address, sample));

                SampleReceived?.Invoke(this,
                    new SourcedSampleReceivedEventArgs(address, sample.DigitalChannels, sample.DigitalSampleState,
                        sample.AnalogChannels, sample.AnalogSamples));
            }
            else if (content is SensorReadIndicatorFrame sensorFrame)
            {
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
            else if (content is RxSmsFrame smsFrame)
            {
                SmsReceived?.Invoke(this, new SmsReceivedEventArgs(smsFrame.PhoneNumber, smsFrame.Message));
            }
            else if (content is RxIPv4Frame ipv4Frame)
            {
                var address = new IPAddress(ipv4Frame.SourceAddress);

                InternetDataReceived?.Invoke(this,
                    new InternetDataReceivedEventArgs(address, ipv4Frame.DestinationPort, ipv4Frame.SourcePort,
                        ipv4Frame.Protocol, ipv4Frame.Data));
            }
            else if (content is NodeIdentificationFrame idFrame)
            {
                var idEvent = new NodeIdentificationEventArgs(
                    new NodeAddress(idFrame.SenderLongAddress, idFrame.SenderShortAddress), 
                    new NodeAddress(idFrame.RemoteLongAddress, idFrame.RemoteShortAddress), 
                    idFrame.ParentAddress, idFrame.Name, idFrame.DeviceType,
                    idFrame.NodeIdentificationReason, idFrame.ReceiveOptions,
                    idFrame.DigiProfileId, idFrame.ManufacturerId);
                NodeIdentificationReceived?.Invoke(this, idEvent);
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
                {
                    _frameId = 1;
                }

                return _frameId;
            }
        }
        
        /// <summary>
        ///     Occurs after a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> FrameMemberSerialized
        {
            add => _serializer.MemberSerialized += value;
            remove => _serializer.MemberSerialized -= value;
        }

        /// <summary>
        ///     Occurs after a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> FrameMemberDeserialized
        {
            add => _serializer.MemberDeserialized += value;
            remove => _serializer.MemberDeserialized -= value;
        }

        /// <summary>
        ///     Occurs before a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> FrameMemberSerializing
        {
            add => _serializer.MemberSerializing += value;
            remove => _serializer.MemberSerializing -= value;
        }

        /// <summary>
        ///     Occurs before a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> FrameMemberDeserializing
        {
            add => _serializer.MemberDeserializing += value;
            remove => _serializer.MemberDeserializing -= value;
        }
    }
}