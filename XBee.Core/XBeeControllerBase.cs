using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BinarySerialization;
using XBee.Frames;
using XBee.Frames.AtCommands;
using XBee.Observable;

// ReSharper disable once CheckNamespace
namespace XBee.Core
{
    public abstract class XBeeControllerBase : IDisposable
    {
        private readonly ConcurrentDictionary<byte, TaskCompletionSource<CommandResponseFrameContent>>
            _executeTaskCompletionSources =
                new ConcurrentDictionary<byte, TaskCompletionSource<CommandResponseFrameContent>>();

        private readonly ConcurrentDictionary<byte, Action<CommandResponseFrameContent>> _executeCallbacks =
            new ConcurrentDictionary<byte, Action<CommandResponseFrameContent>>();

        private static readonly ConcurrentDictionary<NodeAddress, HardwareVersion> NodeHardwareVersionCache =
            new ConcurrentDictionary<NodeAddress, HardwareVersion>();
        private static readonly ConcurrentDictionary<NodeAddress, ushort> FirmwareVersionCache =
            new ConcurrentDictionary<NodeAddress, ushort>();

        private static readonly TimeSpan ModemResetTimeout = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan DefaultRemoteQueryTimeout = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan DefaultLocalQueryTimeout = TimeSpan.FromSeconds(10);

        private static readonly TimeSpan NetworkDiscoveryTimeout = TimeSpan.FromSeconds(10);

        private readonly FrameContext _frameContext = new FrameContext();
        private readonly object _frameIdLock = new object();

        private readonly SemaphoreSlim _initializeSemaphoreSlim = new SemaphoreSlim(1);

        private readonly SemaphoreSlim _listenLock = new SemaphoreSlim(1);
        private readonly SemaphoreSlim _executionLock = new SemaphoreSlim(1);

        private readonly Source<SourcedData> _receivedDataSource = new Source<SourcedData>();
        private readonly Source<SourcedSample> _sampleSource = new Source<SourcedSample>();

        protected readonly ISerialDevice SerialDevice;

        private static readonly BinarySerializer Serializer = new BinarySerializer {Endianness = Endianness.Big};

        private byte _frameId = byte.MinValue;

        private HardwareVersion? _hardwareVersion;
        private ushort? _firmwareVersion;
        private bool _isInitialized;
        private CancellationTokenSource _listenerCancellationTokenSource;
        private TaskCompletionSource<ModemStatus> _modemResetTaskCompletionSource;

        protected XBeeControllerBase(ISerialDevice serialDevice)
        {
            SerialDevice = serialDevice;

#if DEBUG
            Serializer.MemberDeserialized += OnMemberDeserialized;
            Serializer.MemberDeserializing += OnMemberDeserializing;
            Serializer.MemberSerialized += OnMemberSerialized;
            Serializer.MemberSerializing += OnMemberSerializing;
#endif
        }

        /// <summary>
        ///     Get the local node.
        /// </summary>
        public XBeeNode Local { get; private set; }

        public virtual void Dispose()
        {
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
        ///     Occurs when a node identification is received.
        /// </summary>
        public event EventHandler<NodeIdentificationEventArgs> NodeIdentificationReceived;

        /// <summary>
        ///  Occurs when a modem status frame is received.
        /// </summary>
        public event EventHandler<ModemStatusChangedEventArgs> ModemStatusChanged;

        /// <summary>
        ///     Create a node.
        /// </summary>
        /// <param name="address">The address of the node or null for the controller node.</param>
        /// <param name="autodetectProtocol">If true query node for hardware version.  Otherwise assume controller version.</param>
        /// <returns>The specified node.</returns>
        public async Task<XBeeNode> GetNodeAsync(NodeAddress address = null, bool autodetectProtocol = true)
        {
            await InitializeAsync().ConfigureAwait(false);

            if (address == null)
            {
                return Local;
            }

            HardwareVersion hardwareVersion;
            ushort firmwareVersion;

            if (autodetectProtocol)
            {
                hardwareVersion = await
                    TaskExtensions.Retry(async () => await GetHardwareVersionAsync(address), TimeSpan.FromSeconds(5),
                        typeof(TimeoutException), typeof(AtCommandException)).ConfigureAwait(false);

                firmwareVersion = await
                    TaskExtensions.Retry(async () => await GetFirmwareVersionAsync(address), TimeSpan.FromSeconds(5),
                        typeof(TimeoutException), typeof(AtCommandException)).ConfigureAwait(false);
            }
            else
            {
                hardwareVersion = Local.HardwareVersion;
                firmwareVersion = Local.FirmwareVersion;
            }

            return CreateNode(hardwareVersion, firmwareVersion, address);
        }

        private XBeeNode CreateNode(HardwareVersion hardwareVersion, ushort firmwareVersion, NodeAddress address = null)
        {
            return DeviceFactory.CreateDevice(hardwareVersion, firmwareVersion, address, this);
        }

        /// <summary>
        ///     Create a node.
        /// </summary>
        /// <param name="address">The address of the node or null for the controller node.</param>
        /// <param name="version">The hardware version to use for the specified node.</param>
        /// <param name="protocol">The protocol to use or null for default.</param>
        /// <returns>The specified node.</returns>
        public Task<XBeeNode> GetNodeAsync(NodeAddress address, HardwareVersion version, XBeeProtocol protocol = XBeeProtocol.Unknown)
        {
            return Task.FromResult(CreateNode(version, 0, address));
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
        public Task DiscoverNetworkAsync(TimeSpan timeout,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var atCommandFrame = new AtCommandFrameContent(new NetworkDiscoveryCommand());

            return ExecuteMultiQueryAsync(atCommandFrame, new Action<AtCommandResponseFrame>(
                async frame => await OnNodeDiscovered(frame, cancellationToken)), timeout, cancellationToken);
        }

        private async Task OnNodeDiscovered(AtCommandResponseFrame frame, CancellationToken cancellationToken)
        {
            var discoveryData = (NetworkDiscoveryResponseData) frame.Content.Data;

            if (NodeDiscovered == null || discoveryData?.LongAddress == null || discoveryData.IsCoordinator)
            {
                return;
            }

            var address = new NodeAddress(discoveryData.LongAddress, discoveryData.ShortAddress);
            
            // XBees have trouble recovering from discovery
            await Task.Delay(1000, cancellationToken);

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
        }

        /// <summary>
        ///     Occurs after a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> FrameMemberSerialized
        {
            add => Serializer.MemberSerialized += value;
            remove => Serializer.MemberSerialized -= value;
        }

        /// <summary>
        ///     Occurs after a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> FrameMemberDeserialized
        {
            add => Serializer.MemberDeserialized += value;
            remove => Serializer.MemberDeserialized -= value;
        }

        /// <summary>
        ///     Occurs before a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> FrameMemberSerializing
        {
            add => Serializer.MemberSerializing += value;
            remove => Serializer.MemberSerializing -= value;
        }

        /// <summary>
        ///     Occurs before a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> FrameMemberDeserializing
        {
            add => Serializer.MemberDeserializing += value;
            remove => Serializer.MemberDeserializing -= value;
        }

        public async Task<HardwareVersion> GetHardwareVersionAsync(NodeAddress address = null)
        {
            if (address == null && _hardwareVersion != null)
            {
                return _hardwareVersion.Value;
            }

            if (address == null || !NodeHardwareVersionCache.TryGetValue(address, out var hardwareVersion))
            {
                var version =
                    await ExecuteAtQueryAsync<HardwareVersionResponseData>(new HardwareVersionCommand(), address,
                        TimeSpan.FromSeconds(10)).ConfigureAwait(false);

                hardwareVersion = version.HardwareVersion;

                if (address != null)
                {
                    NodeHardwareVersionCache.TryAdd(address, hardwareVersion);
                }
            }

            return hardwareVersion;
        }

        public async Task<ushort> GetFirmwareVersionAsync(NodeAddress address = null)
        {
            if (address == null && _firmwareVersion != null)
            {
                return _firmwareVersion.Value;
            }

            if (address == null || !FirmwareVersionCache.TryGetValue(address, out var firmwareVersion))
            {
                var version =
                    await ExecuteAtQueryAsync<PrimitiveResponseData<ushort>>(new FirmwareVersionCommand(), address,
                        TimeSpan.FromSeconds(10)).ConfigureAwait(false);

                firmwareVersion = version.Value;

                if (address != null)
                {
                    FirmwareVersionCache.TryAdd(address, firmwareVersion);
                }
            }

            return firmwareVersion;
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
                _executeTaskCompletionSources.AddOrUpdate(frame.FrameId,
                    b => new TaskCompletionSource<CommandResponseFrameContent>(),
                    (b, source) => new TaskCompletionSource<CommandResponseFrameContent>());

            await ExecuteAsync(frame);

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
        internal async Task<TResponseFrame> ExecuteQueryAsync<TResponseFrame>(CommandFrameContent frame)
            where TResponseFrame : CommandResponseFrameContent
        {
            await InitializeAsync().ConfigureAwait(false);
            return await ExecuteQueryAsync<TResponseFrame>(frame, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        ///     Send a frame to this node and wait for a response using a default timeout.
        /// </summary>
        /// <typeparam name="TResponseFrame">The expected response type</typeparam>
        /// <param name="frame">The frame to send</param>
        /// <param name="cancellationToken">Used to cancel the operation</param>
        /// <returns>The response frame</returns>
        internal async Task<TResponseFrame> ExecuteQueryAsync<TResponseFrame>(CommandFrameContent frame,
            CancellationToken cancellationToken)
            where TResponseFrame : CommandResponseFrameContent
        {
            await InitializeAsync().ConfigureAwait(false);
            return await ExecuteQueryAsync<TResponseFrame>(frame, DefaultRemoteQueryTimeout, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        ///     Send an AT command to a node and wait for a response.
        /// </summary>
        /// <typeparam name="TResponseData">The expected response data type</typeparam>
        /// <param name="command">The command to send</param>
        /// <param name="address">The address of the node.  If this is null the command will be sent to the local node.</param>
        /// <param name="queueLocal">Queue this command for deferred execution if issued to a local controller.</param>
        /// <returns>The response data</returns>
        internal async Task<TResponseData> ExecuteAtQueryAsync<TResponseData>(AtCommand command,
            NodeAddress address = null, bool queueLocal = false)
            where TResponseData : AtCommandResponseFrameData
        {
            var timeout = address == null ? DefaultLocalQueryTimeout : DefaultRemoteQueryTimeout;
            return await ExecuteAtQueryAsync<TResponseData>(command, address, timeout, queueLocal)
                .ConfigureAwait(false);
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

        internal async Task ResetAsync()
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

        internal async Task ExecuteAsync(FrameContent frameContent)
        {
            await InitializeAsync().ConfigureAwait(false);

            await _executionLock.WaitAsync().ConfigureAwait(false);

            try
            {
                var stream = new SerialDeviceStream(SerialDevice);
                var frame = new Frame(frameContent);
                Serializer.Serialize(stream, frame);
            }
            finally
            {
                _executionLock.Release();
            }
        }

        private async Task InitializeAsync()
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

            await SetApiModeAsync().ConfigureAwait(false);

            try
            {
                // receive one frame to get HW version
                Listen(true);

                // set initialized so GetHardwareVersionAsync doesn't try to enter this again
                _isInitialized = true;

                // Unfortunately the protocol changes based on what type of hardware we're using...
                var hardwareVersion = await GetHardwareVersionAsync().ConfigureAwait(false);

                Listen(true);

                // Unfortunately the protocol changes based on what type of firmware we're using...
                var firmwareVersion = await GetFirmwareVersionAsync().ConfigureAwait(false);

                var protocol = DeviceFactory.GetProtocol(hardwareVersion, firmwareVersion);

                _frameContext.ControllerHardwareVersion = hardwareVersion;
                _frameContext.Protocol = protocol;

                // start receiving frames
                Listen();

                Local = CreateNode(hardwareVersion, firmwareVersion);

                _hardwareVersion = hardwareVersion;
                _firmwareVersion = firmwareVersion;
            }
            catch
            {
                _isInitialized = false;
                throw;
            }
            finally
            {
                _initializeSemaphoreSlim.Release();
            }
        }

        private async Task SetApiModeAsync()
        {
            await _executionLock.WaitAsync().ConfigureAwait(false);

            try
            {
                var stream = new SerialDeviceStream(SerialDevice);
                var writer = new AtCommandWriter(stream);

                // enter command mode
                await writer.WriteAsync("+++").ConfigureAwait(false);

                // enable API mode
                await writer.WriteAsync("ATAP1\r").ConfigureAwait(false);
                
                // leave command mode
                await writer.WriteAsync("ATCN\r").ConfigureAwait(false);
            }
            finally
            {
                _executionLock.Release();
            }
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
            Action<TResponseFrame> callback, TimeSpan timeout, CancellationToken cancellationToken)
            where TResponseFrame : CommandResponseFrameContent
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

            _executeCallbacks.AddOrUpdate(frame.FrameId, b => callbackProxy, (b, source) => callbackProxy);

            await ExecuteAsync(frame).ConfigureAwait(false);

            await Task.Delay(timeout, cancellationToken);

            _executeCallbacks.TryRemove(frame.FrameId, out _);
        }

        private void Listen(bool once = false)
        {
            _listenerCancellationTokenSource = new CancellationTokenSource();

            Task.Run(async () =>
            {
                await _listenLock.WaitAsync();

                try
                {
                    var cancellationToken = _listenerCancellationTokenSource.Token;

                    do
                    {
                        var stream = new SerialDeviceStream(SerialDevice);

                        // ReSharper disable InconsistentlySynchronizedField
                        var frame = await Serializer
                            .DeserializeAsync<Frame>(stream, _frameContext, cancellationToken)
                            .ConfigureAwait(false);
                        // ReSharper restore InconsistentlySynchronizedField

                        ProcessFrame(frame.Payload.Content);

                        // we want to ignore samples and the like...
                        if (once && frame.Payload.Content is CommandResponseFrameContent)
                        {
                            break;
                        }

                    } while (!_listenerCancellationTokenSource.IsCancellationRequested);
                }
                catch (TaskCanceledException)
                {
                }
                finally
                {
                    _listenLock.Release();
                }
            }).ConfigureAwait(false);
        }

        private void ProcessFrame(FrameContent content)
        {
            if (content is ModemStatusFrame modemStatusFrame)
            {
                if (modemStatusFrame.ModemStatus == ModemStatus.HardwareReset)
                {
                    _modemResetTaskCompletionSource?.SetResult(modemStatusFrame.ModemStatus);
                }

                ModemStatusChanged?.Invoke(this, new ModemStatusChangedEventArgs(modemStatusFrame.ModemStatus));
            }
            else if (content is CommandResponseFrameContent commandResponse)
            {
                var frameId = commandResponse.FrameId;

                if (_executeTaskCompletionSources.TryRemove(frameId, out var taskCompletionSource))
                {
                    taskCompletionSource.SetResult(commandResponse);
                }
                else
                {
                    if (_executeCallbacks.TryGetValue(frameId, out var callback))
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
            else
            {
                Debug.WriteLine($"Unsupported frame: {content}");
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

#if DEBUG

        private static void OnMemberSerializing(object sender, MemberSerializingEventArgs e)
        {
            Debug.WriteLine("S-Start: {0} @ {1}", e.MemberName, e.Offset);
        }

        private static void OnMemberSerialized(object sender, MemberSerializedEventArgs e)
        {
            var value = e.Value ?? "null";
            Debug.WriteLine("S-End: {0} ({1}) @ {2}", e.MemberName, value, e.Offset);
        }

        private static void OnMemberDeserializing(object sender, MemberSerializingEventArgs e)
        {
            Debug.WriteLine("D-Start: {0} @ {1}", e.MemberName, e.Offset);
        }

        private static void OnMemberDeserialized(object sender, MemberSerializedEventArgs e)
        {
            var value = e.Value ?? "null";
            Debug.WriteLine("D-End: {0} ({1}) @ {2}", e.MemberName, value, e.Offset);
        }
#endif
    }
}