using System;
using System.Linq;
using System.Threading;
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

            Controller.DataReceived += ControllerOnDataReceived;
            Controller.SampleReceived += ControllerOnSampleReceived;
            Controller.SensorSampleReceived += ControllerOnSensorSampleReceived;
        }

        /// <summary>
        ///     The hardware version for this node.
        /// </summary>
        public HardwareVersion HardwareVersion { get; }

        /// <summary>
        ///     The address of this node.
        /// </summary>
        public virtual NodeAddress Address { get; }

        /// <summary>
        ///     Occurs when data is received from this node.
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        /// <summary>
        ///     Occurs when a sample is received from this node.
        /// </summary>
        public event EventHandler<SampleReceivedEventArgs> SampleReceived;

        /// <summary>
        ///     Occurs when a sample is received from this node.
        /// </summary>
        public event EventHandler<SensorSampleReceivedEventArgs> SensorSampleReceived;

        /// <summary>
        ///     Force a hardware reset of this node.
        /// </summary>
        /// <returns></returns>
        public async Task ResetAsync()
        {
            /* We get no response from remote reset commands */
            await ExecuteAtCommand(new ResetCommand());

            /* Wait approximate reset time per documentation */
            await Task.Delay(HardwareResetTime);
        }

        /// <summary>
        ///     Gets the configured name of this node.
        /// </summary>
        /// <returns>The name of this node</returns>
        public async Task<string> GetNodeIdentifierAsync()
        {
            var response =
                await ExecuteAtQueryAsync<NodeIdentifierResponseData>(new NodeIdentifierCommand());
            return response.Id;
        }

        /// <summary>
        ///     Sets the configured name of this node.
        /// </summary>
        /// <param name="id">The new name for this node</param>
        public async Task SetNodeIdentifierAsync(string id)
        {
            await ExecuteAtCommandAsync(new NodeIdentifierCommand(id));
        }

        /// <summary>
        ///     Get the operating channel used between nodes.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<byte> GetChannelAsync()
        {
            var response =
                await ExecuteAtQueryAsync<PrimitiveResponseData<byte>>(new OperatingChannelCommand());
            return response.Value;
        }

        /// <summary>
        ///     Sets the operating channel used between nodes.
        /// </summary>
        /// <returns></returns>
        public virtual async Task SetChannelAsync(byte channel)
        {
            await ExecuteAtCommandAsync(new OperatingChannelCommand(channel));
        }

        /// <summary>
        ///     Gets the baud rate configured for the serial interface on this node.
        /// </summary>
        /// <returns></returns>
        public async Task<uint> GetBaudRateAsync()
        {
            var response = await ExecuteAtQueryAsync<BaudRateResponseData>(new BaudRateCommand());
            return response.BaudRate;
        }

        /// <summary>
        ///     Sets the baud rate for the serial interface on this node.
        /// </summary>
        /// <param name="baudRate"></param>
        /// <returns></returns>
        public async Task SetBaudRateAsync(BaudRate baudRate)
        {
            await ExecuteAtCommandAsync(new BaudRateCommand(baudRate), true);
        }

        /// <summary>
        ///     Sets a non-standard baud rate for the serial interface on this node.
        /// </summary>
        /// <param name="baudRate"></param>
        /// <returns></returns>
        public async Task SetBaudRateAsync(int baudRate)
        {
            await ExecuteAtCommandAsync(new BaudRateCommand(baudRate), true);
        }

        /// <summary>
        ///     Gets the configured parity for the serial interface on this node.
        /// </summary>
        /// <returns></returns>
        public async Task<Parity> GetParityAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<Parity>>(new ParityCommand());
            return response.Value;
        }

        /// <summary>
        ///     Sets the parity for the serial interface on this node.
        /// </summary>
        /// <param name="parity"></param>
        /// <returns></returns>
        public async Task SetParityAsync(Parity parity)
        {
            await ExecuteAtCommandAsync(new ParityCommand(parity));
        }

        /// <summary>
        ///     Gets the configured number of stop bits for the serial interface on this node.
        /// </summary>
        /// <returns></returns>
        public async Task<StopBits> GetStopBitsAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<StopBits>>(new StopBitsCommand());
            return response.Value;
        }

        /// <summary>
        ///     Sets the number of stop bits for the serial interface on this node.
        /// </summary>
        /// <param name="stopBits"></param>
        /// <returns></returns>
        public async Task SetStopBitsAsync(StopBits stopBits)
        {
            await ExecuteAtCommandAsync(new StopBitsCommand(stopBits));
        }

        /// <summary>
        ///     Gets the number of character times of inter-character silence required before transmission begins when operating in
        ///     Transparent mode.
        /// </summary>
        /// <returns></returns>
        public async Task<byte> GetPacketizationTimeoutAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<byte>>(new PacketizationTimeoutCommand());
            return response.Value;
        }

        /// <summary>
        ///     Sets the number of character times of inter-character silence required before transmission begins when operating in
        ///     Transparent mode.
        /// </summary>
        /// <param name="characterTimes"></param>
        /// <returns></returns>
        public async Task SetPacketizationTimeoutAsync(byte characterTimes)
        {
            await ExecuteAtCommandAsync(new PacketizationTimeoutCommand(characterTimes));
        }

        /// <summary>
        ///     Gets the flow control threshold.
        /// </summary>
        /// <returns></returns>
        public async Task<byte> GetFlowControlThresholdAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<byte>>(new FlowControlThresholdCommand());
            return response.Value;
        }

        /// <summary>
        ///     Sets the flow control threshold.
        /// </summary>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        public async Task SetFlowControlThresholdAsync(byte byteCount)
        {
            await ExecuteAtCommandAsync(new FlowControlThresholdCommand(byteCount));
        }

        /// <summary>
        ///     Get the configured API mode for this node.
        /// </summary>
        /// <returns></returns>
        public async Task<ApiMode> GetApiModeAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<ApiMode>>(new ApiEnableCommand());
            return response.Value;
        }

        /// <summary>
        ///     Set the configured API mode for this node.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public async Task SetApiModeAsync(ApiMode mode)
        {
            await ExecuteAtCommandAsync(new ApiEnableCommand(mode));
        }

        /// <summary>
        ///     Used to explicitly exit command mode.
        /// </summary>
        /// <returns></returns>
        public async Task ExitCommandModeAsync()
        {
            await ExecuteAtCommandAsync(new ExitCommandModeCommand());
        }

        /// <summary>
        ///     Queries the long network address for this node.
        /// </summary>
        /// <returns>The long network address</returns>
        public virtual async Task<NodeAddress> GetAddressAsync()
        {
            var high =
                await ExecuteAtQueryAsync<PrimitiveResponseData<uint>>(new DestinationAddressHighCommand());
            var low =
                await ExecuteAtQueryAsync<PrimitiveResponseData<uint>>(new DestinationAddressLowCommand());

            var address = new LongAddress(high.Value, low.Value);

            // we have to do this nonsense because they decided to reuse "MY" for the cellular IP source address
            var source =
                await ExecuteAtQueryAsync<PrimitiveResponseData<byte[]>>(new SourceAddressCommand());

            var leValue = source.Value.Reverse().ToArray();
            var sourceAddressValue = BitConverter.ToUInt16(leValue, 0);
            var sourceAddress = new ShortAddress(sourceAddressValue);

            return new NodeAddress(address, sourceAddress);
        }

        /// <summary>
        ///     Sets the long network address of this node.
        /// </summary>
        /// <param name="address">The long network address</param>
        public virtual async Task SetDestinationAddressAsync(LongAddress address)
        {
            await ExecuteAtCommandAsync(new DestinationAddressHighCommand(address.High));
            Address.LongAddress.High = address.High;
            await ExecuteAtCommandAsync(new DestinationAddressLowCommand(address.Low));
            Address.LongAddress.Low = address.Low;
        }

        /// <summary>
        ///     Queries the short network address of this node.
        /// </summary>
        /// <param name="address">The short network address</param>
        /// <returns></returns>
        public virtual async Task SetSourceAddressAsync(ShortAddress address)
        {
            await ExecuteAtCommandAsync(new SourceAddressCommand(address));
        }

        /// <summary>
        ///     Gets the static serial number of this node.
        /// </summary>
        /// <returns>The serial number</returns>
        public virtual async Task<LongAddress> GetSerialNumberAsync()
        {
            var highAddress =
                await ExecuteAtQueryAsync<PrimitiveResponseData<uint>>(new SerialNumberHighCommand());
            var lowAddress =
                await ExecuteAtQueryAsync<PrimitiveResponseData<uint>>(new SerialNumberLowCommand());

            return new LongAddress(highAddress.Value, lowAddress.Value);
        }

        /// <summary>
        ///     Gets the configured sleep mode for this node.
        /// </summary>
        /// <returns>The sleep mode</returns>
        public virtual async Task<SleepMode> GetSleepModeAsync()
        {
            var response =
                await ExecuteAtQueryAsync<PrimitiveResponseData<SleepMode>>(new SleepModeCommand());
            return response.Value;
        }

        /// <summary>
        ///     Sets the configured sleep mode for this node.
        /// </summary>
        /// <param name="mode">The sleep mode</param>
        public virtual async Task SetSleepModeAsync(SleepMode mode)
        {
            await ExecuteAtCommandAsync(new SleepModeCommand(mode));
        }

        /// <summary>
        ///     Gets configuration for a channel on this node.
        /// </summary>
        /// <param name="channel">The channel</param>
        /// <returns>The channel configuration</returns>
        public virtual async Task<InputOutputConfiguration> GetInputOutputConfigurationAsync(InputOutputChannel channel)
        {
            var response =
                await ExecuteAtQueryAsync<InputOutputResponseData>(new InputOutputConfigurationCommand(channel));
            return response.Value;
        }

        /// <summary>
        ///     Sets configuration for a channel on this node.
        /// </summary>
        /// <param name="channel">The channel</param>
        /// <param name="configuration">The channel configuration</param>
        public virtual async Task SetInputOutputConfigurationAsync(InputOutputChannel channel,
            InputOutputConfiguration configuration)
        {
            await ExecuteAtCommandAsync(new InputOutputConfigurationCommand(channel, configuration));
        }

        /// <summary>
        ///     Gets channels configured for change detection.
        /// </summary>
        /// <returns>Flags indicating which channels are configured for change detection</returns>
        public virtual async Task<DigitalSampleChannels> GetChangeDetectionChannelsAsync()
        {
            var response =
                await ExecuteAtQueryAsync<InputOutputChangeDetectionResponseData>(
                    new InputOutputChangeDetectionCommand());

            if (response.Channels != null)
            {
                return response.Channels.Value;
            }

            if (response.ChannelsExt != null)
            {
                return response.ChannelsExt.Value;
            }

            throw new InvalidOperationException("No channels returned.");
        }

        /// <summary>
        ///     Sets channels configured for change detection.
        /// </summary>
        /// <param name="channels">Flags indicating which channels to configure for change detection</param>
        /// <returns></returns>
        public virtual async Task SetChangeDetectionChannelsAsync(DigitalSampleChannels channels)
        {
            await ExecuteAtCommandAsync(new InputOutputChangeDetectionCommand(channels));
        }

        /// <summary>
        ///     Force this node to take and report a sample on configured channels.
        /// </summary>
        public virtual async Task ForceSampleAsync()
        {
            await ExecuteAtCommandAsync(new ForceSampleCommand());
        }

        /// <summary>
        ///     Gets the configured sample rate.
        /// </summary>
        /// <returns>The period between samples</returns>
        public virtual async Task<TimeSpan> GetSampleRateAsync()
        {
            var response = await ExecuteAtQueryAsync<SampleRateResponseData>(new SampleRateCommand());
            return response.Interval;
        }

        /// <summary>
        ///     Sets the configured sample rate.
        /// </summary>
        /// <param name="interval">The period between samples</param>
        public virtual async Task SetSampleRateAsync(TimeSpan interval)
        {
            await ExecuteAtCommandAsync(new SampleRateCommand(interval));
        }

        /// <summary>
        ///     Used to determine if encryption is enabled on this node.
        /// </summary>
        /// <returns>True if encryption is enabled</returns>
        public virtual async Task<bool> IsEncryptionEnabledAsync()
        {
            var response =
                await ExecuteAtQueryAsync<PrimitiveResponseData<bool>>(new EncryptionEnableCommand());
            return response.Value;
        }

        /// <summary>
        ///     Used to enable encryption on this node.
        /// </summary>
        /// <param name="enabled">True to enable encryption</param>
        public async Task SetEncryptionEnabledAsync(bool enabled)
        {
            await ExecuteAtCommandAsync(new EncryptionEnableCommand(enabled));
        }

        /// <summary>
        ///     Sets the configured symmetric encryption key for this node.  Only used if encryption is enabled.  There is no way
        ///     to query the configured encryption key.
        /// </summary>
        /// <param name="key">A 16 byte symmetric encryption key</param>
        /// <returns></returns>
        public virtual async Task SetEncryptionKeyAsync(byte[] key)
        {
            await ExecuteAtCommandAsync(new EncryptionKeyCommand(key));
        }

        /// <summary>
        ///     Commit configuration changes to this node.
        /// </summary>
        /// <returns></returns>
        public async Task WriteChangesAsync()
        {
            await ExecuteAtCommandAsync(new WriteCommand());
        }

        /// <summary>
        ///     Subscribe to this node as a sample (not data) source.
        /// </summary>
        /// <returns></returns>
        public virtual IObservable<Sample> GetSamples()
        {
            return Controller.GetSampleSource()
                .Where(sample => sample.Address.Equals(Address))
                .Select(sample => sample.Sample);
        }

        /// <summary>
        ///     Subscribe to this node as a data (not sample) source.
        /// </summary>
        /// <returns></returns>
        public virtual IObservable<byte[]> GetReceivedData()
        {
            return Controller.GetReceivedDataSource()
                .Where(data => data.Address.Equals(Address))
                .Select(data => data.Data);
        }

        /// <summary>
        ///     Send data to this node.  This can either be used in transparent serial mode or to communicate with programmable
        ///     nodes.
        /// </summary>
        /// <param name="data">The data to send</param>
        /// <param name="enableAck">
        ///     True to request an acknowledgment.  If an acknowledgment is requested and no acknowledgment is
        ///     received a TimeoutException will be thrown.
        /// </param>
        public abstract Task TransmitDataAsync(byte[] data, bool enableAck = true);

        /// <summary>
        ///     Send data to this node.  This can either be used in transparent serial mode or to communicate with programmable
        ///     nodes.
        /// </summary>
        /// <param name="data">The data to send</param>
        /// <param name="cancellationToken">Used to cancel the operation</param>
        /// <param name="enableAck">
        ///     True to request an acknowledgment.  If an acknowledgment is requested and no acknowledgment is
        ///     received a TimeoutException will be thrown.
        /// </param>
        public abstract Task TransmitDataAsync(byte[] data, CancellationToken cancellationToken, bool enableAck = true);

        /// <summary>
        ///     Returns a stream that represents serial pass-though on the node.
        /// </summary>
        /// <returns></returns>
        public virtual XBeeStream GetSerialStream()
        {
            return new XBeeStream(this);
        }

        protected virtual NodeAddress GetAddressInternal()
        {
            return Address;
        }

        protected async Task ExecuteAtCommand(AtCommand command)
        {
            await Controller.ExecuteAtCommand(command);
        }

        protected async Task<TResponseData> ExecuteAtQueryAsync<TResponseData>(AtCommand command)
            where TResponseData : AtCommandResponseFrameData
        {
            return await Controller.ExecuteAtQueryAsync<TResponseData>(command, GetAddressInternal());
        }

        protected virtual async Task ExecuteAtCommandAsync(AtCommand command, bool queueLocal = false)
        {
            await Controller.ExecuteAtCommandAsync(command, GetAddressInternal(), queueLocal);
        }

        private void ControllerOnDataReceived(object sender, SourcedDataReceivedEventArgs e)
        {
            if (e.Address.Equals(GetAddressInternal()))
            {
                DataReceived?.Invoke(this, new DataReceivedEventArgs(e.Data));
            }
        }

        private void ControllerOnSampleReceived(object sender, SourcedSampleReceivedEventArgs e)
        {
            if (e.Address.Equals(GetAddressInternal()))
            {
                SampleReceived?.Invoke(this,
                    new SampleReceivedEventArgs(e.DigitalChannels, e.DigitalSampleState, e.AnalogChannels,
                        e.AnalogSamples));
            }
        }

        private void ControllerOnSensorSampleReceived(object sender, SourcedSensorSampleReceivedEventArgs e)
        {
            if (e.Address.Equals(GetAddressInternal()))
            {
                SensorSampleReceived?.Invoke(this,
                    new SensorSampleReceivedEventArgs(e.OneWireSensor, e.SensorValueA, e.SensorValueB,
                        e.SensorValueC,
                        e.SensorValueD, e.TemperatureCelsius));
            }
        }
    }
}