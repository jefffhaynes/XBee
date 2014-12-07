using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
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

        private static readonly SemaphoreSlim OperationLock = new SemaphoreSlim(1);

        private static readonly TimeSpan ModemResetTimeout = TimeSpan.FromMilliseconds(300);
        private static readonly TimeSpan DefaultQueryTimeout = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan NetworkDiscoveryTimeout = TimeSpan.FromSeconds(6);
        private readonly object _frameIdLock = new object();

        private readonly Source<SourcedData> _receivedDataSource = new Source<SourcedData>();
        private readonly Source<SourcedSample> _sampleSource = new Source<SourcedSample>();

        private SerialConnection _connection;
        private byte _frameId = byte.MinValue;
        private TaskCompletionSource<ModemStatus> _modemResetTaskCompletionSource;

        public HardwareVersion HardwareVersion { get; private set; }

        public XBeeNode Local { get; private set; }

        public bool IsOpen
        {
            get { return _connection != null; }
        }

        public void Dispose()
        {
            Close();
            _sampleSource.Dispose();
            _receivedDataSource.Dispose();
        }

        public event EventHandler<NodeDiscoveredEventArgs> NodeDiscovered;

        public event EventHandler<SourcedDataReceivedEventArgs> DataReceived;

        public event EventHandler<SourcedSampleReceivedEventArgs> SampleReceived;

        public async Task OpenAsync(string port, int baudRate)
        {
            if(IsOpen)
                throw new InvalidOperationException("The controller is already conntected, please close the existing connection.");

            _connection = new SerialConnection(port, baudRate);
            _connection.FrameReceived += OnFrameReceived;
            _connection.Open();

            try
            {
                await Reset();
            }
            catch (TimeoutException e)
            {
                throw new InvalidOperationException("Couldn't communicate with device.  Ensure API mode is enabled.", e);
            }

            /* Unfortunately the protocol changes based on what type of hardware we're using... */
            HardwareVersionResponseData response =
                await ExecuteAtQueryAsync<HardwareVersionResponseData>(new HardwareVersionCommand());
            HardwareVersion = response.HardwareVersion;
            _connection.CoordinatorHardwareVersion = HardwareVersion;

            Local = CreateNode(response.HardwareVersion);
        }

        public async Task<XBeeNode> GetRemote(NodeAddress address)
        {
            HardwareVersionResponseData version =
                await ExecuteAtQueryAsync<HardwareVersionResponseData>(new HardwareVersionCommand(), address);
            return CreateNode(version.HardwareVersion, address);
        }

        public async Task Execute(FrameContent frame)
        {
            await _connection.Send(frame);
        }
        
        public async Task ExecuteAtCommand(AtCommand command, NodeAddress address = null)
        {
            if (address == null)
            {
                var atCommandFrame = new AtCommandFrameContent(command);
                await Execute(atCommandFrame);
            }
            else
            {
                var remoteCommand = new RemoteAtCommandFrameContent(address, command);
                await Execute(remoteCommand);
            }
        }

        public async Task<TResponseFrame> ExecuteQueryAsync<TResponseFrame>(CommandFrameContent frame, TimeSpan timeout)
            where TResponseFrame : CommandResponseFrameContent
        {
            await OperationLock.WaitAsync();

            try
            {
                frame.FrameId = GetNextFrameId();

                var delayCancellationTokenSource = new CancellationTokenSource();
                Task delayTask = Task.Delay(timeout, delayCancellationTokenSource.Token);

                TaskCompletionSource<CommandResponseFrameContent> taskCompletionSource =
                    ExecuteTaskCompletionSources.AddOrUpdate(frame.FrameId,
                        b => new TaskCompletionSource<CommandResponseFrameContent>(),
                        (b, source) => new TaskCompletionSource<CommandResponseFrameContent>());

                await Execute(frame);

                if (await Task.WhenAny(taskCompletionSource.Task, delayTask) == taskCompletionSource.Task)
                {
                    delayCancellationTokenSource.Cancel();
                    return await taskCompletionSource.Task as TResponseFrame;
                }

                throw new TimeoutException();
            }
            finally
            {
                OperationLock.Release();
            }
        }

        public Task<TResponseFrame> ExecuteQueryAsync<TResponseFrame>(CommandFrameContent frame)
            where TResponseFrame : CommandResponseFrameContent
        {
            return ExecuteQueryAsync<TResponseFrame>(frame, DefaultQueryTimeout);
        }

        public async Task<TResponseData> ExecuteAtQueryAsync<TResponseData>(AtCommand command,
            NodeAddress address = null)
            where TResponseData : AtCommandResponseFrameData
        {
            AtCommandResponseFrameContent responseContent;

            if (address == null)
            {
                var atCommandFrame = new AtCommandFrameContent(command);
                AtCommandResponseFrame response = await ExecuteQueryAsync<AtCommandResponseFrame>(atCommandFrame);
                responseContent = response.Content;
            }
            else
            {
                var remoteCommand = new RemoteAtCommandFrameContent(address, command);
                RemoteAtCommandResponseFrame response =
                    await ExecuteQueryAsync<RemoteAtCommandResponseFrame>(remoteCommand);
                responseContent = response.Content;
            }

            if (responseContent.Status != AtCommandStatus.Success)
                throw new AtCommandException(responseContent.Status);

            return responseContent.Data as TResponseData;
        }

        public async Task ExecuteAtCommandAsync(AtCommand command, NodeAddress address = null)
        {
            AtCommandResponseFrameContent responseContent;

            if (address == null)
            {
                var atCommandFrame = new AtCommandFrameContent(command);
                AtCommandResponseFrame response = await ExecuteQueryAsync<AtCommandResponseFrame>(atCommandFrame);
                responseContent = response.Content;
            }
            else
            {
                var remoteCommand = new RemoteAtCommandFrameContent(address, command);
                RemoteAtCommandResponseFrame response =
                    await ExecuteQueryAsync<RemoteAtCommandResponseFrame>(remoteCommand);
                responseContent = response.Content;
            }

            if (responseContent.Status != AtCommandStatus.Success)
                throw new AtCommandException(responseContent.Status);
        }

        public async Task ExecuteMultiQueryAsync<TResponseFrame>(CommandFrameContent frame,
            Action<TResponseFrame> callback, TimeSpan timeout) where TResponseFrame : CommandResponseFrameContent
        {
            await OperationLock.WaitAsync();

            try
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

                await Execute(frame);

                await Task.Delay(timeout);

                Action<CommandResponseFrameContent> action;
                ExecuteCallbacks.TryRemove(frame.FrameId, out action);
            }
            finally
            {
                OperationLock.Release();
            }
        }

        public async Task TransmitDataAsync(LongAddress address, byte[] data)
        {
            var transmitRequest = new TxRequestFrame(address, data);
            TxStatusFrame response = await ExecuteQueryAsync<TxStatusFrame>(transmitRequest);

            if (response.Status != DeliveryStatus.Success)
                throw new XBeeException(string.Format("Delivery failed with status code '{0}'.", response.Status));
        }

        public async Task TransmitDataAsync(ShortAddress address, byte[] data)
        {
            var transmitRequest = new TxRequest16Frame(address, data);
            TxStatusFrame response = await ExecuteQueryAsync<TxStatusFrame>(transmitRequest);

            if (response.Status != DeliveryStatus.Success)
                throw new XBeeException(string.Format("Delivery failed with status code '{0}'.", response.Status));
        }

        public async Task TransmitDataExtAsync(LongAddress address, byte[] data)
        {
            var transmitRequest = new TxRequestExtFrame(address, data);
            TxStatusExtFrame response = await ExecuteQueryAsync<TxStatusExtFrame>(transmitRequest);

            if (response.DeliveryStatus != DeliveryStatusExt.Success)
                throw new XBeeException(string.Format("Delivery failed with status code '{0}'.",
                    response.DeliveryStatus));
        }

        public IObservable<SourcedSample> GetSampleSource()
        {
            return _sampleSource;
        }

        public IObservable<SourcedData> GetReceivedDataSource()
        {
            return _receivedDataSource;
        }

        public async Task DiscoverNetwork()
        {
            var atCommandFrame = new AtCommandFrameContent(new NetworkDiscoveryCommand());

            await ExecuteMultiQueryAsync(atCommandFrame, new Action<AtCommandResponseFrame>(
                async frame =>
                {
                    var discoveryData = (NetworkDiscoveryResponseData) frame.Content.Data;

                    if (NodeDiscovered != null && !discoveryData.IsCoordinator)
                    {
                        /* Devices have trouble resetting after ND for some reason... */
                        await Task.Delay(100);

                        var address = new NodeAddress(discoveryData.LongAddress, discoveryData.ShortAddress);
                        XBeeNode node = await GetRemote(address);

                        NodeDiscovered(this,
                            new NodeDiscoveredEventArgs(discoveryData.Name,
                                discoveryData.ReceivedSignalStrengthIndicator.SignalStrength,
                                node));
                    }
                }), NetworkDiscoveryTimeout);
        }

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
                    return new XBeeSeries1(this, HardwareVersion.XBeeProS2, address);
                case HardwareVersion.XBeeProS2B:
                    return new XBeeSeries1(this, HardwareVersion.XBeeProS2B, address);
                case HardwareVersion.XBeePro900:
                    return new XBeePro900HP(this, HardwareVersion.XBeePro900, address);
                case HardwareVersion.XBeePro900HP:
                    return new XBeePro900HP(this, HardwareVersion.XBeePro900HP, address);
                default:
                    throw new NotSupportedException(string.Format("{0} not supported.", hardwareVersion));
            }
        }

        private void OnFrameReceived(object sender, FrameReceivedEventArgs e)
        {
            FrameContent content = e.FrameContent;

            if (content is ModemStatusFrame)
            {
                var modemStatusFrame = content as ModemStatusFrame;

                if (_modemResetTaskCompletionSource != null)
                    _modemResetTaskCompletionSource.SetResult(modemStatusFrame.ModemStatus);
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

                if (DataReceived != null)
                    DataReceived(this, new SourcedDataReceivedEventArgs(address, dataFrame.Data));
            }
            else if (content is IRxIndicatorSampleFrame)
            {
                var sampleFrame = content as IRxIndicatorSampleFrame;
                NodeAddress address = sampleFrame.GetAddress();
                Sample sample = sampleFrame.GetSample();

                _sampleSource.Push(new SourcedSample(address, sample));

                if (SampleReceived != null)
                    SampleReceived(this,
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
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}