using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XBee.Devices;
using XBee.Frames;
using XBee.Frames.AtCommands;

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

        private SerialConnection _connection;
        private byte _frameId = byte.MinValue;
        private TaskCompletionSource<ModemStatus> _modemResetTaskCompletionSource;

        public HardwareVersion HardwareVersion { get; private set; }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        public async Task OpenAsync(string port, int baudRate)
        {
            _connection = new SerialConnection(port, baudRate);
            _connection.FrameReceived += OnFrameReceived;
            _connection.Open();

            await Reset();

            /* Unfortunately the protocol changes based on what type of hardware we're using... */
            HardwareVersionResponseData response =
                await ExecuteAtQueryAsync<HardwareVersionResponseData>(new HardwareVersionCommand());
            HardwareVersion = response.HardwareVersion;
            _connection.CoordinatorHardwareVersion = HardwareVersion;

            Local = CreateNode(response.HardwareVersion);
        }

        public XBeeNode Local { get; private set; }

        public async Task<XBeeNode> GetRemote(NodeAddress address)
        {
            var version = await ExecuteAtQueryAsync<HardwareVersionResponseData>(new HardwareVersionCommand(), address);
            return CreateNode(version.HardwareVersion, address);
        }

        public void Execute(FrameContent frame)
        {
            _connection.Send(frame);
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

                Execute(frame);

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

                Execute(frame);

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
            if (HardwareVersion == HardwareVersion.XBeeSeries1 ||
                HardwareVersion == HardwareVersion.XBeeProSeries1)
            {
                var transmitRequest = new TxRequestFrame(address, data);
                TxStatusFrame response = await ExecuteQueryAsync<TxStatusFrame>(transmitRequest);

                if (response.Status != DeliveryStatus.Success)
                    throw new XBeeException(string.Format("Delivery failed with status code '{0}'.", response.Status));
            }
            else
            {
                var transmitRequest = new TxRequestExtFrame(address, data);
                TxStatusExtFrame response = await ExecuteQueryAsync<TxStatusExtFrame>(transmitRequest);

                if (response.DeliveryStatus != DeliveryStatusExt.Success)
                    throw new XBeeException(string.Format("Delivery failed with status code '{0}'.",
                        response.DeliveryStatus));
            }
        }

        public event EventHandler<NodeDiscoveredEventArgs> NodeDiscovered;

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        public event EventHandler<SampleReceivedEventArgs> SampleReceived;

        public async Task DiscoverNetwork()
        {
            var atCommandFrame = new AtCommandFrameContent(new NetworkDiscoveryCommand());

            await ExecuteMultiQueryAsync(atCommandFrame, new Action<AtCommandResponseFrame>(
                async frame =>
                {
                    var discoveryData = (NetworkDiscoveryResponseData) frame.Content.Data;

                    if (NodeDiscovered != null && !discoveryData.IsCoordinator)
                    {
                        var address = new NodeAddress(discoveryData.LongAddress, discoveryData.ShortAddress);
                        var node = await GetRemote(address);

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

            if (content is CommandResponseFrameContent)
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
            else if (content is ModemStatusFrame)
            {
                var modemStatusFrame = content as ModemStatusFrame;

                if (_modemResetTaskCompletionSource != null)
                    _modemResetTaskCompletionSource.SetResult(modemStatusFrame.ModemStatus);
            }
            else if (content is RxIndicatorExtFrame)
            {
                var rxIndicator = content as RxIndicatorExtFrame;

                if (DataReceived != null)
                    DataReceived(this, new DataReceivedEventArgs(rxIndicator.Source, rxIndicator.Data));
            }
            else if (content is RxIndicatorExplicitExtFrame)
            {
                var rxIndicator = content as RxIndicatorExplicitExtFrame;

                if (DataReceived != null)
                    DataReceived(this, new DataReceivedEventArgs(rxIndicator.Source, rxIndicator.Data));
            }
            else if (content is RxIndicatorSampleFrame)
            {
                var sampleFrame = content as RxIndicatorSampleFrame;
                IEnumerable<SampleChannels> analogChannels =
                    (sampleFrame.Channels & SampleChannels.AllAnalog).GetFlagValues();
                IEnumerable<AnalogSample> analogSamples = sampleFrame.AnalogSamples.Zip(analogChannels,
                    (sample, channel) => new AnalogSample(channel, sample));
                OnSampleReceived(sampleFrame.DigitalSampleState, analogSamples.ToList());
            }
            else if (content is RxIndicator16SampleFrame)
            {
                var sampleFrame = content as RxIndicator16SampleFrame;
                IEnumerable<SampleChannels> analogChannels =
                    (sampleFrame.Channels & SampleChannels.AllAnalog).GetFlagValues();
                IEnumerable<AnalogSample> analogSamples = sampleFrame.AnalogSamples.Zip(analogChannels,
                    (sample, channel) => new AnalogSample(channel, sample));
                OnSampleReceived(sampleFrame.DigitalSampleState, analogSamples.ToList());
            }
            else if (content is RxIndicatorSampleExtFrame)
            {
                var sampleFrame = content as RxIndicatorSampleExtFrame;
                IEnumerable<AnalogSampleChannels> analogChannels =
                    (sampleFrame.AnalogChannels & AnalogSampleChannels.All).GetFlagValues();
                IEnumerable<AnalogSample> analogSamples = sampleFrame.AnalogSamples.Zip(analogChannels,
                    (sample, channel) => new AnalogSample(channel, sample));
                OnSampleReceived(sampleFrame.DigitalSampleState, analogSamples.ToList());
            }
        }

        private byte GetNextFrameId()
        {
            unchecked
            {
                ++_frameId;
            }

            if (_frameId == 0)
                _frameId = 1;

            return _frameId;
        }

        private void OnSampleReceived(DigitalSampleState digitalSampleState, IEnumerable<AnalogSample> analogSamples)
        {
            if (SampleReceived != null)
                SampleReceived(this, new SampleReceivedEventArgs(digitalSampleState, analogSamples.ToList()));
        }
    }
}