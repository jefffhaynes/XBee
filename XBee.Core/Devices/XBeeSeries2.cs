using System.Threading.Tasks;
using XBee.Core;
using XBee.Frames.AtCommands;

namespace XBee.Devices
{
    public class XBeeSeries2 : XBeeSeries2Base
    {
        public XBeeSeries2(XBeeController controller, HardwareVersion hardwareVersion = HardwareVersion.XBeeProS2, NodeAddress address = null) : base(controller, hardwareVersion, address)
        {
        }

        /// <summary>
        ///     Gets the Personal Area Network (PAN) ID.
        /// </summary>
        /// <returns></returns>
        public async Task<ulong> GetPanIdAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<ulong>>(new PanIdCommandExt())
                .ConfigureAwait(false);

            return response.Value;
        }

        /// <summary>
        /// Sets the Personal Area Network (PAN) ID.  To commit changes to non-volatile memory, use <see cref="XBeeNode.WriteChangesAsync"/>.
        /// </summary>
        /// <param name="id">The PAN ID to assign to this node.</param>
        /// <returns></returns>
        public Task SetPanIdAsync(ulong id)
        {
            return ExecuteAtCommandAsync(new PanIdCommandExt(id));
        }

    }
}
