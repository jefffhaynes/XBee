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

        protected readonly XBeeController Controller;

        internal XBeeNode(XBeeController controller, HardwareVersion hardwareVersion, NodeAddress address = null)
        {
            Controller = controller;
            HardwareVersion = hardwareVersion;
            Address = address;

            Controller.SampleReceived += ControllerOnSampleReceived;
            Controller.DataReceived += ControllerOnDataReceived;
        }


        public HardwareVersion HardwareVersion { get; private set; }

        public NodeAddress Address { get; }

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        public event EventHandler<SampleReceivedEventArgs> SampleReceived;


        public async Task Reset()
        {
            /* We get no response from remote reset commands */
            await ExecuteAtCommand(new ResetCommand());

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

        public virtual async Task<NodeAddress> GetDestinationAddress()
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

        public async Task SetDestinationAddress(LongAddress address)
        {
            await ExecuteAtCommandAsync(new DestinationAddressHighCommand(address.High));
            Address.LongAddress.High = address.High;
            await ExecuteAtCommandAsync(new DestinationAddressLowCommand(address.Low));
            Address.LongAddress.Low = address.Low;
        }

        public async Task SetNetworkAddress(ShortAddress address)
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

        public IObservable<Sample> GetSamples()
        {
            return Controller.GetSampleSource()
                .Where(sample => sample.Address.Equals(Address))
                .Select(sample => sample.Sample);
        }

        public IObservable<byte[]> GetReceivedData()
        {
            return Controller.GetReceivedDataSource()
                .Where(data => data.Address.Equals(Address))
                .Select(data => data.Data);
        }

        public abstract Task TransmitDataAsync(byte[] data, bool enableAck = true);

        protected async Task ExecuteAtCommand(AtCommand command)
        {
            await Controller.ExecuteAtCommand(command);
        }

        protected async Task<TResponseData> ExecuteAtQueryAsync<TResponseData>(AtCommand command)
            where TResponseData : AtCommandResponseFrameData
        {
            return await Controller.ExecuteAtQueryAsync<TResponseData>(command, Address);
        }

        protected virtual async Task ExecuteAtCommandAsync(AtCommand command)
        {
            await Controller.ExecuteAtCommandAsync(command, Address);
        }

        private void ControllerOnSampleReceived(object sender, SourcedSampleReceivedEventArgs e)
        {
            if (SampleReceived != null && e.Address.Equals(Address))
                SampleReceived(this, new SampleReceivedEventArgs(e.DigitalSampleState, e.AnalogSamples));
        }

        private void ControllerOnDataReceived(object sender, SourcedDataReceivedEventArgs e)
        {
            if (DataReceived != null && e.Address.Equals(Address))
                DataReceived(this, new DataReceivedEventArgs(e.Data));
        }
    }
}