using System;
using System.Threading;
using System.Threading.Tasks;
using XBee.Frames.AtCommands;

namespace XBee.Devices
{
    public class XBeeCellular : XBeeNode
    {
        public XBeeCellular(XBeeController controller, HardwareVersion hardwareVersion, NodeAddress address = null) : base(controller, hardwareVersion, address)
        {
        }

        public override Task TransmitDataAsync(byte[] data, bool enableAck = true)
        {
            throw new NotImplementedException();
        }

        public override Task TransmitDataAsync(byte[] data, CancellationToken cancellationToken, bool enableAck = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the phone number registered for the inserted SIM.
        /// </summary>
        /// <returns>Phone number as a string</returns>
        public async Task<string> GetPhoneNumberAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<string>>(new PhoneNumberCommand());
            return response.Value;
        }

        /// <summary>
        /// Gets the Integrated Circuit Card Identifier (ICCID) of the inserted SIM.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetIccidAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<string>>(new IccidCommand());
            return response.Value;
        }
    }
}
