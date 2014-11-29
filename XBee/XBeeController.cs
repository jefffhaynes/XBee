using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public HardwareVersion CoordinatorHardwareVersion { get; private set; }

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
            AtCommandResponseFrame response =
                await ExecuteQueryAsync<AtCommandResponseFrame>(new HardwareVersionCommand());
            CoordinatorHardwareVersion = ((HardwareVersionResponseData) response.Content.Data).HardwareVersion;
            _connection.CoordinatorHardwareVersion = CoordinatorHardwareVersion;
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

        public async Task<TResponseData> ExecuteAtQueryAsync<TResponseData>(AtCommandFrame command, LongAddress device = null)
            where TResponseData : AtCommandResponseFrameData
        {
            AtCommandResponseFrameContent responseContent;

            if (device == null)
            {
                AtCommandResponseFrame response = await ExecuteQueryAsync<AtCommandResponseFrame>(command);
                responseContent = response.Content;
            }
            else
            {
                var remoteCommand = new RemoteAtCommandFrame(device, command);
                RemoteAtCommandResponseFrame response = 
                    await ExecuteQueryAsync<RemoteAtCommandResponseFrame>(remoteCommand);
                responseContent = response.Content;
            }

            if (responseContent.Status != AtCommandStatus.Success)
                throw new AtCommandException(responseContent.Status);

            return responseContent.Data as TResponseData;
        }

        public async Task ExecuteAtCommandAsync(AtCommandFrame command, LongAddress device = null)
        {
            AtCommandResponseFrameContent responseContent;

            if (device == null)
            {
                AtCommandResponseFrame response = await ExecuteQueryAsync<AtCommandResponseFrame>(command);
                responseContent = response.Content;
            }
            else
            {
                var remoteCommand = new RemoteAtCommandFrame(device, command);
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
            if (CoordinatorHardwareVersion == HardwareVersion.XBeeSeries1 ||
                CoordinatorHardwareVersion == HardwareVersion.XBeeProSeries1)
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
            await ExecuteMultiQueryAsync(new NetworkDiscoveryCommand(), new Action<AtCommandResponseFrame>(
                frame =>
                {
                    var discoveryData = (NetworkDiscoveryResponseData) frame.Content.Data;

                    if (NodeDiscovered != null && !discoveryData.IsCoordinator)
                        NodeDiscovered(this, new NodeDiscoveredEventArgs(discoveryData.LongAddress,
                            discoveryData.Name, discoveryData.ReceivedSignalStrengthIndicator.SignalStrength));
                }), NetworkDiscoveryTimeout);
        }


        public async Task Reset()
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

        public async Task<bool> IsCoordinator()
        {
            CoordinatorEnableResponseData response;
            if (CoordinatorHardwareVersion == HardwareVersion.XBeePro900HP)
                response = await ExecuteAtQueryAsync<CoordinatorEnableResponseData>(new CoordinatorEnableCommandExt());
            else response = await ExecuteAtQueryAsync<CoordinatorEnableResponseData>(new CoordinatorEnableCommand());

            if (response.EnableState != null)
                return response.EnableState.Value == CoordinatorEnableState.Coordinator;

            if (response.EnableStateExt != null)
                return response.EnableStateExt.Value == CoordinatorEnableStateExt.NonRoutingCoordinator;

            throw new InvalidOperationException("No coordinator state returned.");
        }

        public async Task SetCoordinator(bool enable)
        {
            if (CoordinatorHardwareVersion == HardwareVersion.XBeePro900HP)
                await ExecuteAtCommandAsync(new CoordinatorEnableCommandExt(enable));
            else await ExecuteAtCommandAsync(new CoordinatorEnableCommand(enable));
        }

        public async Task<string> GetNodeIdentification()
        {
            NodeIdentifierResponseData response =
                await ExecuteAtQueryAsync<NodeIdentifierResponseData>(new NodeIdentifierCommand());
            return response.Id;
        }

        public async Task SetNodeIdentifier(string id)
        {
            await ExecuteAtCommandAsync(new NodeIdentifierCommand(id));
        }

        public async Task<LongAddress> GetSerialNumber()
        {
            PrimitiveResponseData<uint> highAddress =
                await ExecuteAtQueryAsync<PrimitiveResponseData<UInt32>>(new SerialNumberHighCommand());
            PrimitiveResponseData<uint> lowAddress =
                await ExecuteAtQueryAsync<PrimitiveResponseData<UInt32>>(new SerialNumberLowCommand());

            return new LongAddress(highAddress.Value, lowAddress.Value);
        }

        public async Task<InputOutputState> GetInputOutputState(InputOutputChannel channel)
        {
            InputOutputResponseData response =
                await ExecuteAtQueryAsync<InputOutputResponseData>(new InputOutputCommand(channel));
            return response.State;
        }

        public async Task SetInputOutputState(InputOutputChannel channel, InputOutputState state)
        {
            await ExecuteAtCommandAsync(new InputOutputCommand(channel, state));
        }

        public async Task<DigitalSampleChannels> GetChangeDetection()
        {
            InputOutputChangeDetectionResponseData response =
                await ExecuteAtQueryAsync<InputOutputChangeDetectionResponseData>(
                        new InputOutputChangeDetectionCommand());
            return response.Channels;
        }

        public async Task SetChangeDetection(DigitalSampleChannels channels)
        {
            await ExecuteAtCommandAsync(new InputOutputChangeDetectionCommand(channels));
        }

        public async Task WriteChanges()
        {
            await ExecuteAtCommandAsync(new WriteCommand());
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