using System.IO;
using System.Threading.Tasks;
using XBee.Core;
using XBee.Frames.AtCommands;

namespace XBee.Devices
{
    public class XBeeSeries2 : XBeeSeries2Base, IAssociationIndicator, IDisassociation
    {
        public XBeeSeries2(XBeeControllerBase controller, 
            HardwareVersion hardwareVersion = HardwareVersion.XBeeProS2,
            ushort firmwareVersion = 0,
            XBeeProtocol protocol = XBeeProtocol.Unknown,
            NodeAddress address = null) : base(controller, hardwareVersion, firmwareVersion, protocol, address)
        {
        }

        /// <summary>
        ///     Gets the Personal Area Network (PAN) ID.
        /// </summary>
        /// <returns></returns>
        public async Task<ulong> GetPanIdAsync()
        {
            var response = await ExecuteAtQueryAsync<PanIdResponseData>(new PanIdCommandExt())
                .ConfigureAwait(false);

            if (response.IdExt != null)
            {
                return response.IdExt.Value;
            }

            if (response.Id != null)
            {
                return response.Id.Value;
            }

            throw new InvalidDataException();
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
        /// Forces device to disassociate with current coordinator and attempt to reassociate.
        /// </summary>
        /// <returns></returns>
        public Task DisassociateAsync()
        {
            return ExecuteAtCommandAsync(new ForceDisassociationCommand());
        }

        /// <summary>
        /// Sets the device product identifier.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task SetDeviceTypeIdentifierAsync(uint type)
        {
            return ExecuteAtCommandAsync(new DeviceTypeIdentifierCommand(type));
        }
    }
}
