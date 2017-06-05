using System;
using System.Collections.Concurrent;
using System.Diagnostics;
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
        private readonly FrameContext _frameContext = new FrameContext(null);
        private readonly object _frameIdLock = new object();

        private readonly SemaphoreSlim _initializeSemaphoreSlim = new SemaphoreSlim(1);

        private readonly SemaphoreSlim _listenLock = new SemaphoreSlim(1);
        private readonly object _executionLock = new object();

        private readonly Source<SourcedData> _receivedDataSource = new Source<SourcedData>();
        private readonly Source<SourcedSample> _sampleSource = new Source<SourcedSample>();

        private readonly ISerialDevice _serialDevice;

        private readonly BinarySerializer _serializer = new BinarySerializer {Endianness = Endianness.Big};

        private byte _frameId = byte.MinValue;

        private HardwareVersion? _hardwareVersion;
        private bool _isInitialized;
        private CancellationTokenSource _listenerCancellationTokenSource;
        private TaskCompletionSource<ModemStatus> _modemResetTaskCompletionSource;

        public XBeeController(ISerialDevice serialDevice)
        {
            _serialDevice = serialDevice;

#if DEBUG
            _serializer.MemberDeserialized += OnMemberDeserialized;
            _serializer.MemberDeserializing += OnMemberDeserializing;
            _serializer.MemberSerialized += OnMemberSerialized;
            _serializer.MemberSerializing += OnMemberSerializing;
#endif
        }

        /// <summary>
        ///     Get the local node.
        /// </summary>
        public XBeeNode Local { get; private set; }

        public void Dispose()
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
        ///     Create a node.
        /// </summary>
        /// <param name="address">The address of the node or null for the controller node.</param>
        /// <param name="autodetectHardwareVersion">If true query node for hardware version.  Otherwise assume controller version.</param>
        /// <returns>The specified node.</returns>
        public async Task<XBeeNode> GetNodeAsync(NodeAddress address = null, bool autodetectHardwareVersion = true)
        {
            await InitializeAsync().ConfigureAwait(false);

            if (address == null)
            {
                return Local;
            }

            HardwareVersion version;

            if (autodetectHardwareVersion)
            {
                version = await
                    TaskExtensions.Retry(async () => await GetHardwareVersionAsync(address), TimeSpan.FromSeconds(5),
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
        public Task DiscoverNetworkAsync(TimeSpan timeout,
            CancellationToken cancellationToken = default(CancellationToken))
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

        public async Task<HardwareVersion> GetHardwareVersionAsync(NodeAddress address = null)
        {
            if (_hardwareVersion != null)
            {
                return _hardwareVersion.Value;
            }

            var version =
                await ExecuteAtQueryAsync<HardwareVersionResponseData>(new HardwareVersionCommand(), address,
                    TimeSpan.FromSeconds(3)).ConfigureAwait(false);

            return version.HardwareVersion;
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

            lock (_executionLock)
            {
                var stream = new SerialDeviceStream(_serialDevice);
                var frame = new Frame(frameContent);
                _serializer.Serialize(stream, frame);
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

            try
            {
                // receive one frame to get HW version
                Listen(true);

                // set initialized so GetHardwareVersionAsync doesn't try to enter this again
                _isInitialized = true;

                // Unfortunately the protocol changes based on what type of hardware we're using...
                var hardwareVersion = await GetHardwareVersionAsync().ConfigureAwait(false);

                _frameContext.ControllerHardwareVersion = hardwareVersion;

                // start receiving frames
                Listen();

                Local = CreateNode(hardwareVersion);

                _hardwareVersion = hardwareVersion;
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

            ExecuteCallbacks.AddOrUpdate(frame.FrameId, b => callbackProxy, (b, source) => callbackProxy);

            await ExecuteAsync(frame).ConfigureAwait(false);

            await Task.Delay(timeout, cancellationToken);

            Action<CommandResponseFrameContent> action;
            ExecuteCallbacks.TryRemove(frame.FrameId, out action);
        }

        private XBeeNode CreateNode(HardwareVersion hardwareVersion, NodeAddress address = null)
        {
            switch (hardwareVersion)
            {
                case HardwareVersion.XBeeSeries1:
                    return new XBeeSeries1(this, HardwareVersion.XBeeSeries1, address);
                case HardwareVersion.XBeeProSeries1:
                    return new XBeeSeries1(this, HardwareVersion.XBeeProSeries1, address);
                case HardwareVersion.ZNetZigBeeS2:
                    return new XBeeSeries2(this, HardwareVersion.ZNetZigBeeS2, address);
                case HardwareVersion.XBeeProS2:
                    return new XBeeSeries2(this, HardwareVersion.XBeeProS2, address);
                case HardwareVersion.XBeeProS2B:
                    return new XBeeSeries2(this, HardwareVersion.XBeeProS2B, address);
                case HardwareVersion.XBee24S2C:
                    return new XBeeSeries2(this, HardwareVersion.XBee24S2C, address);
                case HardwareVersion.XBee24C:
                    return new XBeeSeries2(this, HardwareVersion.XBee24C, address);
                case HardwareVersion.XBeePro24C:
                    return new XBeeSeries2(this, HardwareVersion.XBeePro24C, address);
                case HardwareVersion.XBeePro900:
                    return new XBeePro900HP(this, HardwareVersion.XBeePro900, address);
                case HardwareVersion.XBeePro900HP:
                    return new XBeePro900HP(this, HardwareVersion.XBeePro900HP, address);
                case HardwareVersion.XBeeProSX:
                    return new XBeePro900HP(this, HardwareVersion.XBeeProSX, address);
                case HardwareVersion.XBeeCellular:
                    return new XBeeCellular(this, HardwareVersion.XBeeCellular, address);
                default:
                    throw new NotSupportedException($"Hardware version {hardwareVersion} not supported.");
            }
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
                        var stream = new SerialDeviceStream(_serialDevice);

                        // ReSharper disable InconsistentlySynchronizedField
                        var frame = await _serializer
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
            });
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
    }
}