using System;
using System.Threading;
using System.Threading.Tasks;
using XBee.Frames;
using XBee.Frames.AtCommands;

namespace XBee.Devices
{
    internal class XBeeSeries1 : XBeeNode
    {
        internal XBeeSeries1(XBeeController controller,
            HardwareVersion hardwareVersion = HardwareVersion.XBeeSeries1, 
            NodeAddress address = null) : base(controller, hardwareVersion, address)
        {
        }

        /// <summary>
        /// Gets a value that indicates whether this node is a coordinator node.
        /// </summary>
        /// <returns>True if this is a coordinator node</returns>
        public virtual async Task<bool> IsCoordinatorAsync()
        {
            CoordinatorEnableResponseData response =
                await ExecuteAtQueryAsync<CoordinatorEnableResponseData>(new CoordinatorEnableCommand());

            if(response.EnableState == null)
                throw new InvalidOperationException("No valid coordinator state returned.");

            return response.EnableState.Value == CoordinatorEnableState.Coordinator;
        }
        
        /// <summary>
        /// Sets a value indicating whether this node is a coordinator node.
        /// </summary>
        /// <param name="enable">True if this is a coordinator node</param>
        public virtual async Task SetCoordinatorAsync(bool enable)
        {
            await ExecuteAtCommandAsync(new CoordinatorEnableCommand(enable));
        }

        /// <summary>
        /// Gets flags indicating the configured sleep options for this node.
        /// </summary>
        public async Task<SleepOptions> GetSleepOptionsAsync()
        {
            var response = await ExecuteAtQueryAsync<SleepOptionsResponseData>(new SleepOptionsCommand());

            if(response.Options == null)
                throw new InvalidOperationException("No valid sleep options returned.");

            return response.Options.Value;
        }

        /// <summary>
        /// Sets flags indicating sleep options for this node.
        /// </summary>
        /// <param name="options">Sleep options</param>
        public async Task SetSleepOptionsAsync(SleepOptions options)
        {
            await ExecuteAtCommandAsync(new SleepOptionsCommand(options));
        }

        public override async Task TransmitDataAsync(byte[] data, CancellationToken cancellationToken, bool enableAck = true)
        {
            if (Address == null)
                throw new InvalidOperationException("Can't send data to local device.");

            var transmitRequest = new TxRequestFrame(Address.LongAddress, data);

            if (!enableAck)
            {

                transmitRequest.Options = TransmitOptions.DisableAck;
                await Controller.ExecuteAsync(transmitRequest, cancellationToken);
            }
            else
            {
                TxStatusFrame response = await Controller.ExecuteQueryAsync<TxStatusFrame>(transmitRequest, cancellationToken);

                if (response.Status != DeliveryStatus.Success)
                    throw new XBeeException($"Delivery failed with status code '{response.Status}'.");
            }
        }

        public override async Task TransmitDataAsync(byte[] data, bool enableAck = true)
        {
            await TransmitDataAsync(data, CancellationToken.None, enableAck);
        }

        #region Deprecated
        
        [Obsolete("Use IsCoordinatorAsync")]
        public virtual async Task<bool> IsCoordinator()
        {
            return await IsCoordinatorAsync();
        }
        
        [Obsolete("Use SetCoordinatorAsync")]
        public virtual async Task SetCoordinator(bool enable)
        {
            await SetCoordinatorAsync(enable);
        }

        [Obsolete("Use GetSleepOptionsAsync")]
        public async Task<SleepOptions> GetSleepOptions()
        {
            return await GetSleepOptionsAsync();
        }

        [Obsolete("Use SetSleepOptionsAsync")]
        public async Task SetSleepOptions(SleepOptions options)
        {
            await SetSleepOptionsAsync(options);
        }

        #endregion
    }
}
