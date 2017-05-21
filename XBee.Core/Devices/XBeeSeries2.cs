using System;
using System.Threading;
using System.Threading.Tasks;
using XBee.Frames;
using XBee.Frames.AtCommands;

namespace XBee.Devices
{
    public class XBeeSeries2 : XBeeNode, IAssociationIndicator
    {
        internal XBeeSeries2(XBeeController controller,
            HardwareVersion hardwareVersion = HardwareVersion.XBeeProS2,
            NodeAddress address = null) : base(controller, hardwareVersion, address)
        {
        }

        /// <summary>
        ///     Gets the network association state for this node.
        /// </summary>
        public async Task<AssociationIndicator> GetAssociationAsync()
        {
            var response = await
                Controller.ExecuteAtQueryAsync<PrimitiveResponseData<AssociationIndicator>>(
                    new AssociationIndicationCommand()).ConfigureAwait(false);

            return response.Value;
        }

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

            var transmitRequest = new TxRequestExtFrame(Address.LongAddress, data);

            if (!enableAck)
            {
                transmitRequest.Options = TransmitOptionsExt.DisableAck;
                Controller.ExecuteAsync(transmitRequest);
            }
            else
            {
                var response = await Controller.ExecuteQueryAsync<TxStatusExtFrame>(transmitRequest, cancellationToken)
                    .ConfigureAwait(false);

                if (response.DeliveryStatus != DeliveryStatusExt.Success)
                {
                    throw new XBeeException($"Delivery failed with status code '{response.DeliveryStatus}'.");
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
                throw new XBeeException($"Delivery failed with status code '{response.DeliveryStatus}'.");
            }
        }

        public override Task SetChangeDetectionChannelsAsync(DigitalSampleChannels channels)
        {
            return ExecuteAtCommandAsync(new InputOutputChangeDetectionCommandExt(channels));
        }
    }
}