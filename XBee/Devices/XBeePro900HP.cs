using System;
using System.Threading.Tasks;
using XBee.Frames;
using XBee.Frames.AtCommands;

namespace XBee.Devices
{
    internal class XBeePro900HP : XBeeSeries2
    {
        internal XBeePro900HP(XBeeController controller, 
            HardwareVersion hardwareVersion = HardwareVersion.XBeePro900HP,
            NodeAddress address = null) : base(controller, hardwareVersion, address)
        {
        }

        public override async Task<NodeAddress> GetDestinationAddressAsync()
        {
            var high = await ExecuteAtQueryAsync<PrimitiveResponseData<uint>>(new DestinationAddressHighCommand());
            var low = await ExecuteAtQueryAsync<PrimitiveResponseData<uint>>(new DestinationAddressLowCommand());

            var address = new LongAddress(high.Value, low.Value);

            return new NodeAddress(address);
        }

        /// <summary>
        /// Gets messaging options for this node.
        /// </summary>
        public async Task<NodeMessagingOptions> GetNodeMessagingOptionsAsync()
        {
            CoordinatorEnableResponseData response = 
                await ExecuteAtQueryAsync<CoordinatorEnableResponseData>(new CoordinatorEnableCommandExt());

            if(response.Options == null)
                throw new InvalidOperationException("No valid coordinator state returned.");

            return response.Options.Value;
        }

        /// <summary>
        /// Sets messaging options for this node.
        /// </summary>
        /// <param name="options">Messaging options</param>
        public async Task SetNodeMessagingOptionsAsync(NodeMessagingOptions options)
        {
            await ExecuteAtCommandAsync(new CoordinatorEnableCommandExt(options));
        }

        public override async Task SetChangeDetectionChannelsAsync(DigitalSampleChannels channels)
        {
            await ExecuteAtCommandAsync(new InputOutputChangeDetectionCommandExt(channels));
        }

        public async Task<SleepOptionsExt> GetSleepOptionsAsync()
        {
            var response = await ExecuteAtQueryAsync<SleepOptionsResponseData>(new SleepOptionsCommand());

            if (response.OptionsExt == null)
                throw new InvalidOperationException("No valid sleep options returned.");

            return response.OptionsExt.Value;
        }

        public async Task SetSleepOptionsAsync(SleepOptionsExt options)
        {
            await ExecuteAtCommandAsync(new SleepOptionsCommandExt(options));
        }

        #region Deprecated
        
        [Obsolete("Use GetNodeMessagingOptionsAsync")]
        public async Task<NodeMessagingOptions> GetNodeMessagingOptions()
        {
            return await GetNodeMessagingOptionsAsync();
        }

        [Obsolete("Use SetNodeMessagingOptionsAsync")]
        public async Task SetNodeMessagingOptions(NodeMessagingOptions options)
        {
            await SetNodeMessagingOptionsAsync(options);
        }

        [Obsolete("Use GetSleepOptionsAsync")]
        public async Task<SleepOptionsExt> GetSleepOptions()
        {
            return await GetSleepOptionsAsync();
        }

        [Obsolete("Use SetSleepOptionsAsync")]
        public async Task SetSleepOptions(SleepOptionsExt options)
        {
            await SetSleepOptionsAsync(options);
        }

        #endregion
    }
}
