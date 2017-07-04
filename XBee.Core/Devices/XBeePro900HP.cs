using System;
using System.Threading.Tasks;
using XBee.Core;
using XBee.Frames;
using XBee.Frames.AtCommands;

namespace XBee.Devices
{
    // ReSharper disable once InconsistentNaming
    public class XBeePro900HP : XBeeSeries2Base
    {
        internal XBeePro900HP(XBeeControllerBase controller, 
            HardwareVersion hardwareVersion = HardwareVersion.XBeePro900HP,
            NodeAddress address = null) : base(controller, hardwareVersion, address)
        {
        }

        public override async Task<NodeAddress> GetAddressAsync()
        {
            var high = await ExecuteAtQueryAsync<PrimitiveResponseData<uint>>(new DestinationAddressHighCommand());
            var low = await ExecuteAtQueryAsync<PrimitiveResponseData<uint>>(new DestinationAddressLowCommand());

            var address = new LongAddress(high.Value, low.Value);

            return new NodeAddress(address);
        }

        /// <summary>
        ///     Gets the module Vendor Identification (VID).
        /// </summary>
        /// <returns></returns>
        public async Task<ushort> GetModuleVidAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<ushort>>(new PanIdCommand())
                .ConfigureAwait(false);

            return response.Value;
        }

        /// <summary>
        /// Sets the module Vendor Identification (VID).  To commit changes to non-volatile memory, use <see cref="XBeeNode.WriteChangesAsync"/>.
        /// </summary>
        /// <param name="id">The VID to assign to this node.</param>
        /// <returns></returns>
        public Task SetModuleVidAsync(ushort id)
        {
            return ExecuteAtCommandAsync(new PanIdCommand(id));
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
