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

        public override async Task TransmitDataAsync(byte[] data)
        {
            if (Address == null)
                throw new InvalidOperationException("Can't send data to local device.");

            var transmitRequest = new TxRequestExtFrame(Address.LongAddress, data);
            TxStatusExtFrame response = await Controller.ExecuteQueryAsync<TxStatusExtFrame>(transmitRequest);

            if (response.DeliveryStatus != DeliveryStatusExt.Success)
                throw new XBeeException(string.Format("Delivery failed with status code '{0}'.",
                    response.DeliveryStatus));
        }
    }
}
