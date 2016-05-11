using System;
using System.Threading;
using System.Threading.Tasks;
using XBee.Frames;
using XBee.Frames.AtCommands;

namespace XBee.Devices
{
    public class XBeeSeries2 : XBeeNode
    {
        internal XBeeSeries2(XBeeController controller,
            HardwareVersion hardwareVersion = HardwareVersion.XBeeProS2,
            NodeAddress address = null) : base(controller, hardwareVersion, address)
        {
        }

        public override async Task TransmitDataAsync(byte[] data, bool enableAck = true)
        {
            await TransmitDataAsync(data, CancellationToken.None, enableAck);
        }

        public override async Task TransmitDataAsync(byte[] data, CancellationToken cancellationToken, bool enableAck = true)
        {
            if (Address == null)
                throw new InvalidOperationException("Can't send data to local device.");

            var transmitRequest = new TxRequestExtFrame(Address.LongAddress, data);

            if (!enableAck)
            {
                transmitRequest.Options = TransmitOptionsExt.DisableAck;
                await Controller.ExecuteAsync(transmitRequest, cancellationToken);
            }
            else
            {
                TxStatusExtFrame response = await Controller.ExecuteQueryAsync<TxStatusExtFrame>(transmitRequest, cancellationToken);

                if (response.DeliveryStatus != DeliveryStatusExt.Success)
                    throw new XBeeException($"Delivery failed with status code '{response.DeliveryStatus}'.");
            }
        }

        /// <summary>
        /// Gets the network association state for this node.
        /// </summary>
        public async Task<AssociationIndicator> GetAssociationAsync()
        {
            PrimitiveResponseData<AssociationIndicator> response = await
                    Controller.ExecuteAtQueryAsync<PrimitiveResponseData<AssociationIndicator>>(
                        new AssociationIndicationCommand());

            return response.Value;
        }

        public override async Task SetChangeDetectionChannelsAsync(DigitalSampleChannels channels)
        {
            await ExecuteAtCommandAsync(new InputOutputChangeDetectionCommandExt(channels));
        }

        #region Deprecated
        
        [Obsolete("Use GetAssociationAsync")]
        public async Task<AssociationIndicator> GetAssociation()
        {
            return await GetAssociationAsync();
        }

        #endregion


    }
}