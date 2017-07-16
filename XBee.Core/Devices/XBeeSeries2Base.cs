using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XBee.Core;
using XBee.Frames;
using XBee.Frames.AtCommands;

namespace XBee.Devices
{
    public abstract class XBeeSeries2Base : XBeeNode
    {
        internal XBeeSeries2Base(XBeeControllerBase controller,
            HardwareVersion hardwareVersion = HardwareVersion.XBeeProS2,
            NodeAddress address = null) : base(controller, hardwareVersion, address)
        {
        }

        // cache max payload.  not great but we can't afford to query every time
        // if we're sending lots of data.
        private uint? _maxPayloadLength;

        /// <summary>
        ///     Gets flags indicating the configured sleep options for this node.
        /// </summary>
        public async Task<SleepOptionsExt> GetSleepOptionsAsync()
        {
            var response = await ExecuteAtQueryAsync<SleepOptionsResponseData>(new SleepOptionsCommand())
                .ConfigureAwait(false);

            if (response.OptionsExt == null)
            {
                throw new InvalidOperationException("No valid sleep options returned.");
            }

            return response.OptionsExt.Value;
        }

        /// <summary>
        ///     Sets flags indicating sleep options for this node.
        /// </summary>
        /// <param name="options">Sleep options</param>
        public Task SetSleepOptionsAsync(SleepOptionsExt options)
        {
            return ExecuteAtCommandAsync(new SleepOptionsCommandExt(options));
        }

        /// <summary>
        /// Gets the sleep period.
        /// </summary>
        /// <returns>The sleep period.</returns>
        public async Task<TimeSpan> GetSleepPeriodAsync()
        {
            var response = await ExecuteAtQueryAsync<SleepPeriodResponseData>(new SleepPeriodCommand())
                .ConfigureAwait(false);
            return response.Period;
        }

        /// <summary>
        /// Sets the value for a sleep period.
        /// </summary>
        /// <param name="period">The sleep period.</param>
        /// <returns></returns>
        public Task SetSleepPeriodAsync(TimeSpan period)
        {
            return ExecuteAtCommandAsync(new SleepPeriodCommand(period));
        }

        public async Task<ushort> GetSleepPeriodCount()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<ushort>>(new SleepPeriodCountCommand())
                .ConfigureAwait(false);
            return response.Value;
        }

        public Task SetSleepPeriodCount(ushort periodCount)
        {
            return ExecuteAtCommandAsync(new SleepPeriodCountCommand(periodCount));
        }

        /// <summary>
        /// Gets the configured pull-up resistor values.
        /// </summary>
        public async Task<PullUpResistorConfigurationExt> GetPullUpResistorConfigurationAsync()
        {
            var response =
                await ExecuteAtQueryAsync<PullUpResistorConfigurationResponseData>(
                    new PullUpResistorConfigurationCommand());
            return response.ConfigurationExt.GetValueOrDefault();
        }

        /// <summary>
        /// Sets the active pull-up resistors.
        /// </summary>
        /// <param name="configuration">A set of flags specifying the active pull-ups.</param>
        public Task SetPullUpResistorConfigurationAsync(PullUpResistorConfigurationExt configuration)
        {
            return ExecuteAtCommandAsync(new PullUpResistorConfigurationCommand(configuration));
        }

        /// <summary>
        /// Get the maximum support payload length.  This value could vary based on device settings such
        /// as security or routing settings.
        /// </summary>
        /// <returns></returns>
        public override async Task<uint> GetMaximumTransmitPayloadLengthAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<ushort>>(new MaximumPayloadBytesCommand());
            return response.Value;
        }

        public override async Task TransmitDataAsync(byte[] data, bool enableAck = true)
        {
            await TransmitDataAsync(data, CancellationToken.None, enableAck);
        }

        public override async Task TransmitDataAsync(byte[] data, CancellationToken cancellationToken,
            bool enableAck = true)
        {
            if (Address == null)
            {
                throw new InvalidOperationException("Can't send data to local device.");
            }

            if (_maxPayloadLength == null)
            {
                _maxPayloadLength = await GetMaximumTransmitPayloadLengthAsync();
            }

            var dataStream = new MemoryStream(data);

            int read;
            // ReSharper disable once PossibleInvalidOperationException
            byte[] block = new byte[_maxPayloadLength.Value];
            while ((read = dataStream.Read(block, 0, block.Length)) > 0)
            {
                var readBlock = new byte[read];
                Array.Copy(block, readBlock, readBlock.Length);
                var transmitRequest = new TxRequestExtFrame(Address.LongAddress, Address.ShortAddress, data);

                if (!enableAck)
                {
                    transmitRequest.Options = TransmitOptionsExt.DisableAck;
                    await Controller.ExecuteAsync(transmitRequest).ConfigureAwait(false);
                }
                else
                {
                    var response = await Controller
                        .ExecuteQueryAsync<TxStatusExtFrame>(transmitRequest, cancellationToken)
                        .ConfigureAwait(false);

                    if (response.DeliveryStatus != DeliveryStatusExt.Success)
                    {
                        // per documentation, set short address to unknown on failure
                        Address.ShortAddress = ShortAddress.Disabled;

                        throw new XBeeException($"Delivery failed with status code '{response.DeliveryStatus}'.");
                    }

                    // per documention, update short address
                    Address.ShortAddress = response.ShortAddress;
                }
            }
        }

        public async Task TransmitDataAsync(byte[] data, CancellationToken cancellationToken, byte sourceEndpoint,
            byte destinationEndpoint, ushort clusterId, ushort profileId)
        {
            if (Address == null)
            {
                throw new InvalidOperationException("Can't send data to local device.");
            }

            var transmitRequest = new TxRequestExplicitFrame(Address.LongAddress, data)
            {
                SourceEndpoint = sourceEndpoint,
                DestinationEndpoint = destinationEndpoint,
                ClusterId = clusterId,
                ProfileId = profileId
            };

            var response =
                await Controller.ExecuteQueryAsync<TxStatusExtFrame>(transmitRequest, cancellationToken)
                    .ConfigureAwait(false);

            if (response.DeliveryStatus != DeliveryStatusExt.Success)
            {
                // per documentation, set short address to unknown on failure
                Address.ShortAddress = ShortAddress.Disabled;

                throw new XBeeException($"Delivery failed with status code '{response.DeliveryStatus}'.");
            }

            // per documention, update short address
            Address.ShortAddress = response.ShortAddress;
        }

        public override Task SetChangeDetectionChannelsAsync(DigitalSampleChannels channels)
        {
            return ExecuteAtCommandAsync(new InputOutputChangeDetectionCommandExt(channels));
        }

        protected override void OnMaxPayloadLengthDirty()
        {
            _maxPayloadLength = null;
        }
    }
}