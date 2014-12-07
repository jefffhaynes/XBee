using System;
using System.Threading.Tasks;
using XBee.Frames;
using XBee.Frames.AtCommands;
using XBee.Observable;

namespace XBee
{
    public abstract class XBeeNode
    {
        private static readonly TimeSpan HardwareResetTime = TimeSpan.FromMilliseconds(200);

        private readonly XBeeController _controller;
        private readonly Source<Sample> _sampleSource = new Source<Sample>();

        internal XBeeNode(XBeeController controller, HardwareVersion hardwareVersion, NodeAddress address = null)
        {
            _controller = controller;
            HardwareVersion = hardwareVersion;
            Address = address;
        }

        public HardwareVersion HardwareVersion { get; private set; }

        public NodeAddress Address { get; private set; }

        //public event EventHandler<DataReceivedEventArgs> DataReceived;

        //public event EventHandler<SampleReceivedEventArgs> SampleReceived;


        public async Task Reset()
        {
            /* We get no response from remote reset commands */
            ExecuteAtCommand(new ResetCommand());

            /* Wait approximate reset time per documentation */
            await Task.Delay(HardwareResetTime);
        }

        public async Task<string> GetNodeIdentifier()
        {
            NodeIdentifierResponseData response =
                await ExecuteAtQueryAsync<NodeIdentifierResponseData>(new NodeIdentifierCommand());
            return response.Id;
        }

        public async Task SetNodeIdentifier(string id)
        {
            await ExecuteAtCommandAsync(new NodeIdentifierCommand(id));
        }

        public virtual async Task<NodeAddress> GetAddress()
        {
            PrimitiveResponseData<uint> high =
                await ExecuteAtQueryAsync<PrimitiveResponseData<uint>>(new DestinationAddressHighCommand());
            PrimitiveResponseData<uint> low =
                await ExecuteAtQueryAsync<PrimitiveResponseData<uint>>(new DestinationAddressLowCommand());

            var address = new LongAddress(high.Value, low.Value);

            PrimitiveResponseData<ShortAddress> source =
                await ExecuteAtQueryAsync<PrimitiveResponseData<ShortAddress>>(new SourceAddressCommand());

            return new NodeAddress(address, source.Value);
        }

        public async Task SetAddress(LongAddress address)
        {
            await ExecuteAtCommandAsync(new DestinationAddressHighCommand(address.High));
            Address.LongAddress.High = address.High;
            await ExecuteAtCommandAsync(new DestinationAddressLowCommand(address.Low));
            Address.LongAddress.Low = address.Low;
        }

        public async Task SetAddress(ShortAddress address)
        {
            await ExecuteAtCommandAsync(new SourceAddressCommand(address));
        }

        public async Task<LongAddress> GetSerialNumber()
        {
            PrimitiveResponseData<uint> highAddress =
                await ExecuteAtQueryAsync<PrimitiveResponseData<UInt32>>(new SerialNumberHighCommand());
            PrimitiveResponseData<uint> lowAddress =
                await ExecuteAtQueryAsync<PrimitiveResponseData<UInt32>>(new SerialNumberLowCommand());

            return new LongAddress(highAddress.Value, lowAddress.Value);
        }

        public virtual async Task<SleepMode> GetSleepMode()
        {
            PrimitiveResponseData<SleepMode> response =
                await ExecuteAtQueryAsync<PrimitiveResponseData<SleepMode>>(new SleepModeCommand());
            return response.Value;
        }

        public virtual async Task SetSleepMode(SleepMode mode)
        {
            await ExecuteAtCommandAsync(new SleepModeCommand(mode));
        }

        public async Task<InputOutputConfiguration> GetInputOutputConfiguration(InputOutputChannel channel)
        {
            InputOutputResponseData response =
                await ExecuteAtQueryAsync<InputOutputResponseData>(new InputOutputConfigurationCommand(channel));
            return response.Value;
        }

        public async Task SetInputOutputConfiguration(InputOutputChannel channel, InputOutputConfiguration configuration)
        {
            await ExecuteAtCommandAsync(new InputOutputConfigurationCommand(channel, configuration));
        }

        public async Task<DigitalSampleChannels> GetChangeDetectionChannels()
        {
            InputOutputChangeDetectionResponseData response =
                await ExecuteAtQueryAsync<InputOutputChangeDetectionResponseData>(
                    new InputOutputChangeDetectionCommand());

            if (response.Channels != null)
                return response.Channels.Value;

            if (response.ChannelsExt != null)
                return response.ChannelsExt.Value;

            throw new InvalidOperationException("No channels returned.");
        }

        public virtual async Task SetChangeDetectionChannels(DigitalSampleChannels channels)
        {
            await ExecuteAtCommandAsync(new InputOutputChangeDetectionCommand(channels));
        }

        public async Task ForceSample()
        {
            await ExecuteAtCommandAsync(new ForceSampleCommand());
        }

        public async Task<TimeSpan> GetSampleRate()
        {
            SampleRateResponseData response = await ExecuteAtQueryAsync<SampleRateResponseData>(new SampleRateCommand());
            return response.Interval;
        }

        public async Task SetSampleRate(TimeSpan interval)
        {
            await ExecuteAtCommandAsync(new SampleRateCommand(interval));
        }

        public async Task<bool> IsEncryptionEnabled()
        {
            PrimitiveResponseData<bool> response =
                await ExecuteAtQueryAsync<PrimitiveResponseData<bool>>(new EncryptionEnableCommand());
            return response.Value;
        }

        public async Task SetEncryptionEnabled(bool enabled)
        {
            await ExecuteAtCommandAsync(new EncryptionEnableCommand(enabled));
        }

        public async Task SetEncryptionKey(byte[] key)
        {
            await ExecuteAtCommandAsync(new EncryptionKeyCommand(key));
        }

        public async Task WriteChanges()
        {
            await ExecuteAtCommandAsync(new WriteCommand());
        }

        protected void ExecuteAtCommand(AtCommand command)
        {
            _controller.ExecuteAtCommand(command);
        }

        protected async Task<TResponseData> ExecuteAtQueryAsync<TResponseData>(AtCommand command)
            where TResponseData : AtCommandResponseFrameData
        {
            return await _controller.ExecuteAtQueryAsync<TResponseData>(command, Address);
        }

        protected virtual async Task ExecuteAtCommandAsync(AtCommand command)
        {
            await _controller.ExecuteAtCommandAsync(command, Address);
        }

        public IObservable<Sample> GetSamples()
        {
            return _controller.GetSampleSource().Where(sample => sample.Address.Equals(Address)).Select(sample => sample.Sample);
        }
    }
}