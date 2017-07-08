using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XBee.Core;
using XBee.Frames;
using XBee.Frames.AtCommands;

namespace XBee.Devices
{
    public class XBeeSeries1 : XBeeNode, IAssociationIndicator, IDisassociation
    {
        internal XBeeSeries1(XBeeControllerBase controller,
            HardwareVersion hardwareVersion = HardwareVersion.XBeeSeries1,
            NodeAddress address = null) : base(controller, hardwareVersion, address)
        {
        }

        private const uint MaxTransmitPayloadLength = 100;
        private const uint MaxTransmitPayloadLengthWithSecurity = 95;

        private uint? _maxPayloadLength;

        /// <summary>
        ///     Gets a value that indicates whether this node is a coordinator node.
        /// </summary>
        /// <returns>True if this is a coordinator node</returns>
        public virtual async Task<bool> IsCoordinatorAsync()
        {
            var response =
                await ExecuteAtQueryAsync<CoordinatorEnableResponseData>(new CoordinatorEnableCommand())
                    .ConfigureAwait(false);

            if (response.EnableState == null)
            {
                throw new InvalidOperationException("No valid coordinator state returned.");
            }

            return response.EnableState.Value == CoordinatorEnableState.Coordinator;
        }

        /// <summary>
        ///     Sets a value indicating whether this node is a coordinator node.
        /// </summary>
        /// <param name="enable">True if this is a coordinator node</param>
        public virtual Task SetCoordinatorAsync(bool enable)
        {
            return ExecuteAtCommandAsync(new CoordinatorEnableCommand(enable));
        }

        /// <summary>
        ///     Gets the Personal Area Network (PAN) ID.
        /// </summary>
        /// <returns></returns>
        public async Task<ushort> GetPanIdAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<ushort>>(new PanIdCommand())
                .ConfigureAwait(false);

            return response.Value;
        }

        /// <summary>
        /// Sets the Personal Area Network (PAN) ID.  To commit changes to non-volatile memory, use <see cref="XBeeNode.WriteChangesAsync"/>.
        /// </summary>
        /// <param name="id">The PAN ID to assign to this node.</param>
        /// <returns></returns>
        public Task SetPanIdAsync(ushort id)
        {
            return ExecuteAtCommandAsync(new PanIdCommand(id));
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
        /// Gets end device association options.
        /// </summary>
        /// <returns></returns>
        public async Task<EndDeviceAssociationOptions> GetEndDeviceAssociationOptionsAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<EndDeviceAssociationOptions>>(
                new EndDeviceAssociationOptionsCommand()).ConfigureAwait(false);
            return response.Value;
        }

        /// <summary>
        /// Sets end device association options.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task SetEndDeviceAssociationOptionsAsync(EndDeviceAssociationOptions options)
        {
            return ExecuteAtCommandAsync(new EndDeviceAssociationOptionsCommand(options));
        }

        /// <summary>
        /// Gets coordinator association options.
        /// </summary>
        /// <returns></returns>
        public async Task<CoordinatorAssociationOptions> GetCoordinatorAssociationOptionsAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<CoordinatorAssociationOptions>>(
                new CoordinatorAssociationOptionsCommand()).ConfigureAwait(false);
            return response.Value;
        }

        /// <summary>
        /// Sets coordinator association options.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task SetCoordinatorAssociationOptionsAsync(CoordinatorAssociationOptions options)
        {
            return ExecuteAtCommandAsync(new CoordinatorAssociationOptionsCommand(options));
        }

        /// <summary>
        ///     Gets flags indicating the configured sleep options for this node.
        /// </summary>
        public async Task<SleepOptions> GetSleepOptionsAsync()
        {
            var response = await ExecuteAtQueryAsync<SleepOptionsResponseData>(new SleepOptionsCommand())
                .ConfigureAwait(false);

            if (response.Options == null)
            {
                throw new InvalidOperationException("No valid sleep options returned.");
            }

            return response.Options.Value;
        }

        /// <summary>
        ///     Sets flags indicating sleep options for this node.
        /// </summary>
        /// <param name="options">Sleep options</param>
        public Task SetSleepOptionsAsync(SleepOptions options)
        {
            return ExecuteAtCommandAsync(new SleepOptionsCommand(options));
        }

        /// <summary>
        /// Gets the configured pull-up resistor values.
        /// </summary>
        public async Task<PullUpResistorConfiguration> GetPullUpResistorConfigurationAsync()
        {
            var response =
                await ExecuteAtQueryAsync<PullUpResistorConfigurationResponseData>(
                    new PullUpResistorConfigurationCommand());

            return response == null ? PullUpResistorConfiguration.None : response.Configuration.GetValueOrDefault();
        }

        /// <summary>
        /// Sets the active pull-up resistors.
        /// </summary>
        /// <param name="configuration">A set of flags specifying the active pull-ups.</param>
        public Task SetPullUpResistorConfigurationAsync(PullUpResistorConfiguration configuration)
        {
            return ExecuteAtCommandAsync(new PullUpResistorConfigurationCommand(configuration));
        }

        /// <summary>
        /// Get the maximum support payload length.  This value could vary based on device settings such
        /// as security or routing settings.
        /// </summary>
        /// <returns></returns>
        public override async Task<uint> GetMaximumTransmitPayloadLengthAsync()
        {
            var isEncryptionEnabled = await IsEncryptionEnabledAsync().ConfigureAwait(false);

            var maxPayloadLength = isEncryptionEnabled ? MaxTransmitPayloadLengthWithSecurity : MaxTransmitPayloadLength;

            return maxPayloadLength;
        }

        public override async Task TransmitDataAsync(byte[] data, CancellationToken cancellationToken,
            bool enableAck = true)
        {
            if (Address == null)
            {
                throw new InvalidOperationException("Can't send data to local device.");
            }
            
            if (_maxPayloadLength == null)
            {
                _maxPayloadLength = await GetMaximumTransmitPayloadLengthAsync();
            }
            
            var dataStream = new MemoryStream(data);

            int read;
            // ReSharper disable once PossibleInvalidOperationException
            byte[] block = new byte[_maxPayloadLength.Value];
            while ((read = dataStream.Read(block, 0, block.Length)) > 0)
            {
                var readBlock = new byte[read];
                Array.Copy(block, readBlock, readBlock.Length);
                var transmitRequest = new TxRequestFrame(Address.LongAddress, readBlock);
                
                if (!enableAck)
                {
                    transmitRequest.Options = TransmitOptions.DisableAck;
                    await Controller.ExecuteAsync(transmitRequest).ConfigureAwait(false);
                }
                else
                {
                    var response = await Controller.ExecuteQueryAsync<TxStatusFrame>(transmitRequest, cancellationToken)
                        .ConfigureAwait(false);

                    if (response.Status != DeliveryStatus.Success)
                    {
                        throw new XBeeException($"Delivery failed with status code '{response.Status}'.");
                    }
                }
            }
        }

        public override Task TransmitDataAsync(byte[] data, bool enableAck = true)
        {
            return TransmitDataAsync(data, CancellationToken.None, enableAck);
        }

        protected override void OnMaxPayloadLengthDirty()
        {
            _maxPayloadLength = null;
        }
    }
}