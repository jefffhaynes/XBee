using System.Threading.Tasks;
using XBee.Frames;
using XBee.Frames.AtCommands;

namespace XBee.Devices
{
    internal class XBeePro900HP : XBeeNode
    {
        internal XBeePro900HP(XBeeController controller, 
            HardwareVersion hardwareVersion = HardwareVersion.XBeePro900HP,
            NodeAddress address = null) : base(controller, hardwareVersion, address)
        {
        }

        public override async Task<NodeAddress> GetAddress()
        {
            var high = await ExecuteAtQueryAsync<PrimitiveResponseData<uint>>(new DestinationAddressHighCommand());
            var low = await ExecuteAtQueryAsync<PrimitiveResponseData<uint>>(new DestinationAddressLowCommand());

            var address = new LongAddress(high.Value, low.Value);

            return new NodeAddress(address);
        }

        public override async Task<bool> IsCoordinator()
        {
            CoordinatorEnableResponseData response = 
                await ExecuteAtQueryAsync<CoordinatorEnableResponseData>(new CoordinatorEnableCommandExt());

            return response.IsCoordinator;
        }

        public override async Task SetCoordinator(bool enable)
        {
            await ExecuteAtCommandAsync(new CoordinatorEnableCommandExt(enable));
        }

        public override async Task SetChangeDetection(DigitalSampleChannels channels)
        {
            await ExecuteAtCommandAsync(new InputOutputChangeDetectionCommandExt(channels));
        }
    }
}
