using System;
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
            if (Address == null)
                throw new InvalidOperationException("Can't send data to local device.");

            var transmitRequest = new TxRequestExtFrame(Address.LongAddress, data);

            if (!enableAck)
            {
                transmitRequest.Options = TransmitOptionsExt.DisableAck;
                await Controller.ExecuteAsync(transmitRequest);
            }
            else
            {
                TxStatusExtFrame response = await Controller.ExecuteQueryAsync<TxStatusExtFrame>(transmitRequest);

                if (response.DeliveryStatus != DeliveryStatusExt.Success)
                    throw new XBeeException(string.Format("Delivery failed with status code '{0}'.",
                        response.DeliveryStatus));
            }
        }

        public async Task<AssociationIndicator> GetAssociation()
        {
            PrimitiveResponseData<AssociationIndicator> response = await
                    Controller.ExecuteAtQueryAsync<PrimitiveResponseData<AssociationIndicator>>(
                        new AssociationIndicationCommand());

            return response.Value;
        }
    }
}