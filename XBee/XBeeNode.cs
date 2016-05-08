using System;
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
        /// The hardware version for this node.
        /// </summary>
        public HardwareVersion HardwareVersion { get; private set; }

        /// <summary>
        /// The address of this node.
        /// </summary>
        public NodeAddress Address { get; }

        /// <summary>
        /// Occurs when data is received from this node.
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        /// <summary>
        /// Occurs when a sample is received from this node.
        /// </summary>
        public event EventHandler<SampleReceivedEventArgs> SampleReceived;
        
        /// <summary>
        /// Occurs when a sample is received from this node.
        /// </summary>
        public event EventHandler<SensorSampleReceivedEventArgs> SensorSampleReceived;

        /// <summary>
        /// Force a hardware reset of this node.
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
        /// Gets the configured name of this node.
        /// </summary>
        /// <returns>The name of this node</returns>
        public async Task<string> GetNodeIdentifierAsync()
        {
            NodeIdentifierResponseData response =
                await ExecuteAtQueryAsync<NodeIdentifierResponseData>(new NodeIdentifierCommand());
            return response.Id;
        }

        /// <summary>
        /// Sets the configured name of this node.
        /// </summary>
        /// <param name="id">The new name for this node</param>
        public async Task SetNodeIdentifierAsync(string id)
        {
            await ExecuteAtCommandAsync(new NodeIdentifierCommand(id));
        }

        /// <summary>
        /// Queries the long network address for this node.
        /// </summary>
        /// <returns>The long network address</returns>
        public virtual async Task<NodeAddress> GetDestinationAddressAsync()
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

        /// <summary>
        /// Sets the long network address of this node.
        /// </summary>
        /// <param name="address">The long network address</param>
        public async Task SetDestinationAddressAsync(LongAddress address)
        {
            await ExecuteAtCommandAsync(new DestinationAddressHighCommand(address.High));
            Address.LongAddress.High = address.High;
            await ExecuteAtCommandAsync(new DestinationAddressLowCommand(address.Low));
            Address.LongAddress.Low = address.Low;
        }
        
        /// <summary>
        /// Queries the short network address of this node.
        /// </summary>
        /// <param name="address">The short network address</param>
        /// <returns></returns>
        public async Task SetNetworkAddressAsync(ShortAddress address)
        {
            await ExecuteAtCommandAsync(new SourceAddressCommand(address));
        }

        /// <summary>
        /// Gets the static serial number of this node.
        /// </summary>
        /// <returns>The serial number</returns>
        public async Task<LongAddress> GetSerialNumberAsync()
        {
            PrimitiveResponseData<uint> highAddress =
                await ExecuteAtQueryAsync<PrimitiveResponseData<UInt32>>(new SerialNumberHighCommand());
            PrimitiveResponseData<uint> lowAddress =
                await ExecuteAtQueryAsync<PrimitiveResponseData<UInt32>>(new SerialNumberLowCommand());

            return new LongAddress(highAddress.Value, lowAddress.Value);
        }

        /// <summary>
        /// Gets the configured sleep mode for this node.
        /// </summary>
        /// <returns>The sleep mode</returns>
        public virtual async Task<SleepMode> GetSleepModeAsync()
        {
            PrimitiveResponseData<SleepMode> response =
                await ExecuteAtQueryAsync<PrimitiveResponseData<SleepMode>>(new SleepModeCommand());
            return response.Value;
        }

        /// <summary>
        /// Sets the configured sleep mode for this node.
        /// </summary>
        /// <param name="mode">The sleep mode</param>
        public virtual async Task SetSleepModeAsync(SleepMode mode)
        {
            await ExecuteAtCommandAsync(new SleepModeCommand(mode));
        }

        /// <summary>
        /// Gets configuration for a channel on this node.
        /// </summary>
        /// <param name="channel">The channel</param>
        /// <returns>The channel configuration</returns>
        public async Task<InputOutputConfiguration> GetInputOutputConfigurationAsync(InputOutputChannel channel)
        {
            InputOutputResponseData response =
                await ExecuteAtQueryAsync<InputOutputResponseData>(new InputOutputConfigurationCommand(channel));
            return response.Value;
        }

        /// <summary>
        /// Sets configuration for a channel on this node.
        /// </summary>
        /// <param name="channel">The channel</param>
        /// <param name="configuration">The channel configuration</param>
        public async Task SetInputOutputConfigurationAsync(InputOutputChannel channel, InputOutputConfiguration configuration)
        {
            await ExecuteAtCommandAsync(new InputOutputConfigurationCommand(channel, configuration));
        }

        /// <summary>
        /// Gets channels configured for change detection.
        /// </summary>
        /// <returns>Flags indicating which channels are configured for change detection</returns>
        public async Task<DigitalSampleChannels> GetChangeDetectionChannelsAsync()
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

        /// <summary>
        /// Sets channels configured for change detection.
        /// </summary>
        /// <param name="channels">Flags indicating which channels to configure for change detection</param>
        /// <returns></returns>
        public virtual async Task SetChangeDetectionChannelsAsync(DigitalSampleChannels channels)
        {
            await ExecuteAtCommandAsync(new InputOutputChangeDetectionCommand(channels));
        }

        /// <summary>
        /// Force this node to take and report a sample on configured channels.
        /// </summary>
        public async Task ForceSampleAsync()
        {
            await ExecuteAtCommandAsync(new ForceSampleCommand());
        }

        /// <summary>
        /// Gets the configured sample rate.
        /// </summary>
        /// <returns>The period between samples</returns>
        public async Task<TimeSpan> GetSampleRateAsync()
        {
            SampleRateResponseData response = await ExecuteAtQueryAsync<SampleRateResponseData>(new SampleRateCommand());
            return response.Interval;
        }

        /// <summary>
        /// Sets the configured sample rate.
        /// </summary>
        /// <param name="interval">The period between samples</param>
        public async Task SetSampleRateAsync(TimeSpan interval)
        {
            await ExecuteAtCommandAsync(new SampleRateCommand(interval));
        }

        /// <summary>
        /// Used to determine if encryption is enabled on this node.
        /// </summary>
        /// <returns>True if encryption is enabled</returns>
        public async Task<bool> IsEncryptionEnabledAsync()
        {
            PrimitiveResponseData<bool> response =
                await ExecuteAtQueryAsync<PrimitiveResponseData<bool>>(new EncryptionEnableCommand());
            return response.Value;
        }

        /// <summary>
        /// Used to enable encryption on this node.
        /// </summary>
        /// <param name="enabled">True to enable encryption</param>
        public async Task SetEncryptionEnabledAsync(bool enabled)
        {
            await ExecuteAtCommandAsync(new EncryptionEnableCommand(enabled));
        }

        /// <summary>
        /// Sets the configured symmetric encryption key for this node.  Only used if encryption is enabled.  There is no way to query the configured encryption key.
        /// </summary>
        /// <param name="key">A 16 byte symmetric encryption key</param>
        /// <returns></returns>
        public async Task SetEncryptionKeyAsync(byte[] key)
        {
            await ExecuteAtCommandAsync(new EncryptionKeyCommand(key));
        }

        /// <summary>
        /// Commit configuration changes to this node.
        /// </summary>
        /// <returns></returns>
        public async Task WriteChangesAsync()
        {
            await ExecuteAtCommandAsync(new WriteCommand());
        }

        /// <summary>
        /// Subscribe to this node as a sample (not data) source.
        /// </summary>
        /// <returns></returns>
        public IObservable<Sample> GetSamples()
        {
            return Controller.GetSampleSource()
                .Where(sample => sample.Address.Equals(Address))
                .Select(sample => sample.Sample);
        }

        /// <summary>
        /// Subscribe to this node as a data (not sample) source.
        /// </summary>
        /// <returns></returns>
        public IObservable<byte[]> GetReceivedData()
        {
            return Controller.GetReceivedDataSource()
                .Where(data => data.Address.Equals(Address))
                .Select(data => data.Data);
        }

        /// <summary>
        /// Send data to this node.  This can either be used in transparent serial mode or to communicate with programmable nodes.
        /// </summary>
        /// <param name="data">The data to send</param>
        /// <param name="enableAck">True to request an acknowledgement.  If an acknowledgement is requested and no acknowledgement is received a TimeoutException will be thrown.</param>
        public abstract Task TransmitDataAsync(byte[] data, bool enableAck = true);

        /// <summary>
        /// Send data to this node.  This can either be used in transparent serial mode or to communicate with programmable nodes.
        /// </summary>
        /// <param name="data">The data to send</param>
        /// <param name="cancellationToken">Used to cancel the operation</param>
        /// <param name="enableAck">True to request an acknowledgement.  If an acknowledgement is requested and no acknowledgement is received a TimeoutException will be thrown.</param>
        public abstract Task TransmitDataAsync(byte[] data, CancellationToken cancellationToken, bool enableAck = true);

        /// <summary>
        /// Returns a stream that represents serial passthough on the node.
        /// </summary>
        /// <returns></returns>
        public XBeeStream GetSerialStream()
        {
            return new XBeeStream(this);
        }

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

        private void ControllerOnDataReceived(object sender, SourcedDataReceivedEventArgs e)
        {
            if (DataReceived != null && e.Address.Equals(Address))
                DataReceived(this, new DataReceivedEventArgs(e.Data));
        }
        
        private void ControllerOnSampleReceived(object sender, SourcedSampleReceivedEventArgs e)
        {
            if (SampleReceived != null && e.Address.Equals(Address))
                SampleReceived(this, new SampleReceivedEventArgs(e.DigitalChannels, e.DigitalSampleState, e.AnalogChannels, e.AnalogSamples));
        }

        private void ControllerOnSensorSampleReceived(object sender, SourcedSensorSampleReceivedEventArgs e)
        {
            if (SensorSampleReceived != null && e.Address.Equals(Address))
                SensorSampleReceived(this,
                    new SensorSampleReceivedEventArgs(e.OneWireSensor, e.SensorValueA, e.SensorValueB, e.SensorValueC,
                        e.SensorValueD, e.TemperatureCelsius));
        }

        #region Deprecated
        
        [Obsolete("Use ResetAsync")]
        public async Task Reset()
        {
            await ResetAsync();
        }
        
        [Obsolete("Use GetNodeIdentifierAsync")]
        public async Task<string> GetNodeIdentifier()
        {
            return await GetNodeIdentifierAsync();
        }
        
        [Obsolete("Use SetNodeIdentifierAsync")]
        public async Task SetNodeIdentifier(string id)
        {
            await SetNodeIdentifierAsync(id);
        }

        [Obsolete("Use GetDestinationAddressAsync")]
        public virtual async Task<NodeAddress> GetDestinationAddress()
        {
            return await GetDestinationAddressAsync();
        }

        [Obsolete("Use SetDestinationAddressAsync")]
        public async Task SetDestinationAddress(LongAddress address)
        {
            await SetDestinationAddressAsync(address);
        }

        [Obsolete("Use SetNetworkAddressAsync")]
        public async Task SetNetworkAddress(ShortAddress address)
        {
            await SetNetworkAddressAsync(address);
        }

        [Obsolete("Use GetSerialNumberAsync")]
        public async Task<LongAddress> GetSerialNumber()
        {
            return await GetSerialNumberAsync();
        }
        
        [Obsolete("Use GetSleepModeAsync")]
        public virtual async Task<SleepMode> GetSleepMode()
        {
            return await GetSleepModeAsync();
        }
        
        [Obsolete("Use SetSleepModeAsync")]
        public virtual async Task SetSleepMode(SleepMode mode)
        {
            await SetSleepModeAsync(mode);
        }
        
        [Obsolete("Use GetInputOutputConfigurationAsync")]
        public async Task<InputOutputConfiguration> GetInputOutputConfiguration(InputOutputChannel channel)
        {
            return await GetInputOutputConfigurationAsync(channel);
        }

        [Obsolete("Use SetInputOutputConfigurationAsync")]
        public async Task SetInputOutputConfiguration(InputOutputChannel channel, InputOutputConfiguration configuration)
        {
            await SetInputOutputConfigurationAsync(channel, configuration);
        }

        [Obsolete("Use GetChangeDetectionChannelsAsync")]
        public async Task<DigitalSampleChannels> GetChangeDetectionChannels()
        {
            return await GetChangeDetectionChannelsAsync();
        }

        [Obsolete("Use SetChangeDetectionChannelsAsync")]
        public virtual async Task SetChangeDetectionChannels(DigitalSampleChannels channels)
        {
            await SetChangeDetectionChannelsAsync(channels);
        }
        
        [Obsolete("Use ForceSampleAsync")]
        public async Task ForceSample()
        {
            await ForceSampleAsync();
        }
        
        [Obsolete("Use GetSampleRateAsync")]
        public async Task<TimeSpan> GetSampleRate()
        {
            return await GetSampleRateAsync();
        }

        [Obsolete("Use SetSampleRateAsync")]
        public async Task SetSampleRate(TimeSpan interval)
        {
            await SetSampleRateAsync(interval);
        }
        
        [Obsolete("Use IsEncryptionEnabledAsync")]
        public async Task<bool> IsEncryptionEnabled()
        {
            return await IsEncryptionEnabledAsync();
        }
        
        [Obsolete("Use SetEncryptionEnabledAsync")]
        public async Task SetEncryptionEnabled(bool enabled)
        {
            await SetEncryptionEnabledAsync(enabled);
        }

        [Obsolete("Use SetEncryptionKeyAsync")]
        public async Task SetEncryptionKey(byte[] key)
        {
            await SetEncryptionKeyAsync(key);
        }
        
        [Obsolete("Use WriteChangesAsync")]
        public async Task WriteChanges()
        {
            await WriteChangesAsync();
        }

        #endregion
    }
}