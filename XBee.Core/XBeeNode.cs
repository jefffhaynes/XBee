using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XBee.Core;
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
            await ExecuteAtCommandAsync(new ResetCommand()).ConfigureAwait(false);

            /* Wait approximate reset time per documentation */
            await Task.Delay(HardwareResetTime).ConfigureAwait(false);
        }

        /// <summary>
        ///     Gets the configured name of this node.
        /// </summary>
        /// <returns>The name of this node</returns>
        public async Task<string> GetNodeIdentifierAsync()
        {
            var response =
                await ExecuteAtQueryAsync<NodeIdentifierResponseData>(new NodeIdentifierCommand())
                    .ConfigureAwait(false);
            return response.Id;
        }

        /// <summary>
        ///     Sets the configured name of this node.
        /// </summary>
        /// <param name="id">The new name for this node</param>
        public Task SetNodeIdentifierAsync(string id)
        {
            return ExecuteAtCommandAsync(new NodeIdentifierCommand(id));
        }

        /// <summary>
        ///     Get the operating channel used between nodes.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<byte> GetChannelAsync()
        {
            var response =
                await ExecuteAtQueryAsync<PrimitiveResponseData<byte>>(new OperatingChannelCommand())
                    .ConfigureAwait(false);
            return response.Value;
        }

        /// <summary>
        ///     Sets the operating channel used between nodes.
        /// </summary>
        /// <returns></returns>
        public virtual Task SetChannelAsync(byte channel)
        {
            return ExecuteAtCommandAsync(new OperatingChannelCommand(channel));
        }

        /// <summary>
        ///     Gets the baud rate configured for the serial interface on this node.
        /// </summary>
        /// <returns></returns>
        public async Task<uint> GetBaudRateAsync()
        {
            var response = await ExecuteAtQueryAsync<BaudRateResponseData>(new BaudRateCommand()).ConfigureAwait(false);
            return response.BaudRate;
        }

        /// <summary>
        ///     Sets the baud rate for the serial interface on this node.
        /// </summary>
        /// <param name="baudRate"></param>
        /// <returns></returns>
        public Task SetBaudRateAsync(BaudRate baudRate)
        {
            return ExecuteAtCommandAsync(new BaudRateCommand(baudRate), true);
        }

        /// <summary>
        ///     Sets a non-standard baud rate for the serial interface on this node.
        /// </summary>
        /// <param name="baudRate"></param>
        /// <returns></returns>
        public Task SetBaudRateAsync(int baudRate)
        {
            return ExecuteAtCommandAsync(new BaudRateCommand(baudRate), true);
        }

        /// <summary>
        ///     Gets the configured parity for the serial interface on this node.
        /// </summary>
        /// <returns></returns>
        public async Task<Parity> GetParityAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<Parity>>(new ParityCommand())
                .ConfigureAwait(false);
            return response.Value;
        }

        /// <summary>
        ///     Sets the parity for the serial interface on this node.
        /// </summary>
        /// <param name="parity"></param>
        /// <returns></returns>
        public Task SetParityAsync(Parity parity)
        {
            return ExecuteAtCommandAsync(new ParityCommand(parity));
        }

        /// <summary>
        ///     Gets the configured number of stop bits for the serial interface on this node.
        /// </summary>
        /// <returns></returns>
        public async Task<StopBits> GetStopBitsAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<StopBits>>(new StopBitsCommand())
                .ConfigureAwait(false);
            return response.Value;
        }

        /// <summary>
        ///     Sets the number of stop bits for the serial interface on this node.
        /// </summary>
        /// <param name="stopBits"></param>
        /// <returns></returns>
        public Task SetStopBitsAsync(StopBits stopBits)
        {
            return ExecuteAtCommandAsync(new StopBitsCommand(stopBits));
        }

        /// <summary>
        ///     Gets the number of character times of inter-character silence required before transmission begins when operating in
        ///     Transparent mode.
        /// </summary>
        /// <returns></returns>
        public async Task<byte> GetPacketizationTimeoutAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<byte>>(new PacketizationTimeoutCommand())
                .ConfigureAwait(false);
            return response.Value;
        }

        /// <summary>
        ///     Sets the number of character times of inter-character silence required before transmission begins when operating in
        ///     Transparent mode.
        /// </summary>
        /// <param name="characterTimes"></param>
        /// <returns></returns>
        public Task SetPacketizationTimeoutAsync(byte characterTimes)
        {
            return ExecuteAtCommandAsync(new PacketizationTimeoutCommand(characterTimes));
        }

        /// <summary>
        ///     Gets the flow control threshold.
        /// </summary>
        /// <returns></returns>
        public async Task<byte> GetFlowControlThresholdAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<byte>>(new FlowControlThresholdCommand())
                .ConfigureAwait(false);
            return response.Value;
        }

        /// <summary>
        ///     Sets the flow control threshold.
        /// </summary>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        public Task SetFlowControlThresholdAsync(byte byteCount)
        {
            return ExecuteAtCommandAsync(new FlowControlThresholdCommand(byteCount));
        }

        /// <summary>
        ///     Get the configured API mode for this node.
        /// </summary>
        /// <returns></returns>
        public async Task<ApiMode> GetApiModeAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<ApiMode>>(new ApiEnableCommand())
                .ConfigureAwait(false);
            return response.Value;
        }

        /// <summary>
        ///     Set the configured API mode for this node.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public Task SetApiModeAsync(ApiMode mode)
        {
            return ExecuteAtCommandAsync(new ApiEnableCommand(mode));
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
                await ExecuteAtQueryAsync<PrimitiveResponseData<uint>>(new DestinationAddressHighCommand())
                    .ConfigureAwait(false);
            var low =
                await ExecuteAtQueryAsync<PrimitiveResponseData<uint>>(new DestinationAddressLowCommand())
                    .ConfigureAwait(false);

            var address = new LongAddress(high.Value, low.Value);

            // we have to do this nonsense because they decided to reuse "MY" for the cellular IP source address
            var source =
                await ExecuteAtQueryAsync<PrimitiveResponseData<byte[]>>(new SourceAddressCommand())
                    .ConfigureAwait(false);

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
            await ExecuteAtCommandAsync(new DestinationAddressHighCommand(address.High)).ConfigureAwait(false);
            Address.LongAddress.High = address.High;
            await ExecuteAtCommandAsync(new DestinationAddressLowCommand(address.Low)).ConfigureAwait(false);
            Address.LongAddress.Low = address.Low;
        }

        /// <summary>
        ///     Queries the short network address of this node.
        /// </summary>
        /// <param name="address">The short network address</param>
        /// <returns></returns>
        public virtual Task SetSourceAddressAsync(ShortAddress address)
        {
            return ExecuteAtCommandAsync(new SourceAddressCommand(address));
        }

        /// <summary>
        ///     Gets the static serial number of this node.
        /// </summary>
        /// <returns>The serial number</returns>
        public virtual async Task<LongAddress> GetSerialNumberAsync()
        {
            var highAddress =
                await ExecuteAtQueryAsync<PrimitiveResponseData<uint>>(new SerialNumberHighCommand())
                    .ConfigureAwait(false);
            var lowAddress =
                await ExecuteAtQueryAsync<PrimitiveResponseData<uint>>(new SerialNumberLowCommand())
                    .ConfigureAwait(false);

            return new LongAddress(highAddress.Value, lowAddress.Value);
        }

        /// <summary>
        /// Sets the network discovery timeout used by this node when it is acting as a coordinator.  This
        /// value will be included in discovery requests and used by the responding nodes to randomly
        /// choose a delay before responding.  Note that this value is independent of any timeout value
        /// specified when calling <see cref="XBeeController.DiscoverNetworkAsync(TimeSpan,CancellationToken)"/>.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public virtual Task SetNetworkDiscoveryTimeoutAsync(TimeSpan timeout)
        {
            return ExecuteAtCommandAsync(new NetworkDiscoveryTimeoutCommand(timeout));
        }

        /// <summary>
        /// Gets the network discovery timeout used by this node when it is acting as a coordinator.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<TimeSpan> GetNetworkDiscoveryAsync()
        {
            var response =
                await ExecuteAtQueryAsync<NetworkDiscoveryTimeoutResponseData>(new NetworkDiscoveryTimeoutCommand());

            return response.Timeout;
        }

        /// <summary>
        ///     Gets the configured sleep mode for this node.
        /// </summary>
        /// <returns>The sleep mode</returns>
        public virtual async Task<SleepMode> GetSleepModeAsync()
        {
            var response =
                await ExecuteAtQueryAsync<PrimitiveResponseData<SleepMode>>(new SleepModeCommand())
                    .ConfigureAwait(false);
            return response.Value;
        }

        /// <summary>
        ///     Sets the configured sleep mode for this node.
        /// </summary>
        /// <param name="mode">The sleep mode</param>
        public virtual Task SetSleepModeAsync(SleepMode mode)
        {
            return ExecuteAtCommandAsync(new SleepModeCommand(mode));
        }

        /// <summary>
        /// Gets the Recevied Signal Strength Indicator (RSSI) Pulse Width Modulation (PWM) timer value.
        /// </summary>
        /// <returns>The duration for which the RSSI PWM will go high after a receive event.</returns>
        public virtual async Task<TimeSpan> GetRssiPwmTimeAsync()
        {
            var response = await ExecuteAtQueryAsync<RssiPwmTimeResponseData>(new RssiPwmTimeCommand());
            return response.Timeout;
        }

        /// <summary>
        /// Gets the Recevied Signal Strength Indicator (RSSI) Pulse Width Modulation (PWM) timer raw value.
        /// </summary>
        /// <returns>The raw value that represents the duration for which the RSSI PWM will go high after a receive event.</returns>
        public virtual async Task<byte> GetRssiPwmTimeValueAsync()
        {
            var response = await ExecuteAtQueryAsync<RssiPwmTimeResponseData>(new RssiPwmTimeCommand());
            return response.TimeoutValue;
        }

        /// <summary>
        /// Sets the Recevied Signal Strength Indicator (RSSI) Pulse Width Modulation (PWM) timer value.
        /// </summary>
        /// <param name="timeout">The duration for which the RSSI PWM will go high after a receive event.</param>
        /// <returns></returns>
        public virtual Task SetRssiPwmTimeAsync(TimeSpan timeout)
        {
            return ExecuteAtCommandAsync(new RssiPwmTimeCommand(timeout));
        }

        /// <summary>
        /// Sets the Recevied Signal Strength Indicator (RSSI) Pulse Width Modulation (PWM) timer value.
        /// </summary>
        /// <param name="value">The value which represents the duration for which the RSSI PWM will go high after a receive event.</param>
        /// <returns></returns>
        public virtual Task SetRssiPwmTimeAsync(byte value)
        {
            return ExecuteAtCommandAsync(new RssiPwmTimeCommand(value));
        }

        /// <summary>
        ///     Gets configuration for a channel on this node.
        /// </summary>
        /// <param name="channel">The channel</param>
        /// <returns>The channel configuration</returns>
        public virtual async Task<InputOutputConfiguration> GetInputOutputConfigurationAsync(InputOutputChannel channel)
        {
            var response =
                await ExecuteAtQueryAsync<InputOutputResponseData>(new InputOutputConfigurationCommand(channel))
                    .ConfigureAwait(false);
            return response.Value;
        }

        /// <summary>
        ///     Sets configuration for a channel on this node.
        /// </summary>
        /// <param name="channel">The channel</param>
        /// <param name="configuration">The channel configuration</param>
        public virtual Task SetInputOutputConfigurationAsync(InputOutputChannel channel,
            InputOutputConfiguration configuration)
        {
            return ExecuteAtCommandAsync(new InputOutputConfigurationCommand(channel, configuration));
        }

        /// <summary>
        ///     Gets channels configured for change detection.
        /// </summary>
        /// <returns>Flags indicating which channels are configured for change detection</returns>
        public virtual async Task<DigitalSampleChannels> GetChangeDetectionChannelsAsync()
        {
            var response =
                await ExecuteAtQueryAsync<InputOutputChangeDetectionResponseData>(
                    new InputOutputChangeDetectionCommand()).ConfigureAwait(false);

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
        public virtual Task SetChangeDetectionChannelsAsync(DigitalSampleChannels channels)
        {
            return ExecuteAtCommandAsync(new InputOutputChangeDetectionCommand(channels));
        }

        /// <summary>
        ///     Force this node to take and report a sample on configured channels.
        /// </summary>
        public virtual Task ForceSampleAsync()
        {
            return ExecuteAtCommandAsync(new ForceSampleCommand());
        }

        /// <summary>
        /// Restore module parameters to factory defaults.
        /// </summary>
        /// <returns></returns>
        public virtual Task RestoreDefaultsAsync()
        {
            return ExecuteAtCommandAsync(new RestoreDefaultsCommand());
        }

        /// <summary>
        ///     Gets the configured sample rate.
        /// </summary>
        /// <returns>The period between samples</returns>
        public virtual async Task<TimeSpan> GetSampleRateAsync()
        {
            var response = await ExecuteAtQueryAsync<SampleRateResponseData>(new SampleRateCommand())
                .ConfigureAwait(false);
            return response.Interval;
        }

        /// <summary>
        ///     Sets the configured sample rate.
        /// </summary>
        /// <param name="interval">The period between samples</param>
        public virtual Task SetSampleRateAsync(TimeSpan interval)
        {
            return ExecuteAtCommandAsync(new SampleRateCommand(interval));
        }

        /// <summary>
        ///     Used to determine if encryption is enabled on this node.
        /// </summary>
        /// <returns>True if encryption is enabled</returns>
        public virtual async Task<bool> IsEncryptionEnabledAsync()
        {
            var response =
                await ExecuteAtQueryAsync<PrimitiveResponseData<bool>>(new EncryptionEnableCommand())
                    .ConfigureAwait(false);
            return response.Value;
        }

        /// <summary>
        ///     Used to enable encryption on this node.
        /// </summary>
        /// <param name="enabled">True to enable encryption</param>
        public Task SetEncryptionEnabledAsync(bool enabled)
        {
            return ExecuteAtCommandAsync(new EncryptionEnableCommand(enabled));
        }

        /// <summary>
        ///     Sets the configured symmetric encryption key for this node.  Only used if encryption is enabled.  There is no way
        ///     to query the configured encryption key.
        /// </summary>
        /// <param name="key">A 16 byte symmetric encryption key</param>
        /// <returns></returns>
        public virtual Task SetEncryptionKeyAsync(byte[] key)
        {
            return ExecuteAtCommandAsync(new EncryptionKeyCommand(key));
        }

        /// <summary>
        /// Gets the transmit power level for this module.  Refer to documentation for dBm equivelent.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<byte> GetPowerLevelValueAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<byte>>(new PowerLevelCommand());
            return response.Value;
        }

        /// <summary>
        /// Sets the transmit power level for this module.  Refer to documentation for dBm equivelent.
        /// </summary>
        /// <param name="powerLevel"></param>
        /// <returns></returns>
        public virtual Task SetPowerLevelValueAsync(byte powerLevel)
        {
            return ExecuteAtCommandAsync(new PowerLevelCommand(powerLevel));
        }

        /// <summary>
        ///     Commit configuration changes to this node.
        /// </summary>
        /// <returns></returns>
        public Task WriteChangesAsync()
        {
            return ExecuteAtCommandAsync(new WriteCommand());
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

        internal Task<TResponseData> ExecuteAtQueryAsync<TResponseData>(AtCommand command)
            where TResponseData : AtCommandResponseFrameData
        {
            return Controller.ExecuteAtQueryAsync<TResponseData>(command, GetAddressInternal());
        }

        internal virtual Task ExecuteAtCommandAsync(AtCommand command, bool queueLocal = false)
        {
            return Controller.ExecuteAtCommandAsync(command, GetAddressInternal(), queueLocal);
        }

        private void ControllerOnDataReceived(object sender, SourcedDataReceivedEventArgs e)
        {
            if (e.Address.Equals(GetAddressInternal()))
            {
                DataReceived?.Invoke(this, new DataReceivedEventArgs(Address, e.Data));
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