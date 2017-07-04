using System;
using System.Threading.Tasks;
using XBee.Core;
using XBee.Frames;
using XBee.Frames.AtCommands;

namespace XBee
{
    public abstract class XBeeNodeBase
    {
        private static readonly TimeSpan HardwareResetTime = TimeSpan.FromMilliseconds(200);
        protected XBeeControllerBase Controller;

        protected XBeeNodeBase(XBeeControllerBase controller, HardwareVersion hardwareVersion, NodeAddress address = null)
        {
            Controller = controller;
            HardwareVersion = hardwareVersion;
            Address = address;
        }

        /// <summary>
        /// The hardware version for this node.
        /// </summary>
        public HardwareVersion HardwareVersion { get; private set; }

        /// <summary>
        /// The address of this node.
        /// </summary>
        public NodeAddress Address { get; }

        /// <summary>
        /// Force a hardware reset of this node.
        /// </summary>
        /// <returns></returns>
        public async Task ResetAsync()
        {
            /* We get no response from remote reset commands */
            await ExecuteAtCommandAsync(new ResetCommand()).ConfigureAwait(false);

            /* Wait approximate reset time per documentation */
            await Task.Delay(HardwareResetTime).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the configured name of this node.
        /// </summary>
        /// <returns>The name of this node</returns>
        public async Task<string> GetNodeIdentifierAsync()
        {
            NodeIdentifierResponseData response =
                await ExecuteAtQueryAsync<NodeIdentifierResponseData>(new NodeIdentifierCommand());
            return response.Id;
        }

        /// <summary>
        /// Sets the configured name of this node.
        /// </summary>
        /// <param name="id">The new name for this node</param>
        public async Task SetNodeIdentifierAsync(string id)
        {
            await ExecuteAtCommandAsync(new NodeIdentifierCommand(id));
        }

        /// <summary>
        /// Get the baud rate configured for the serial interface on this node.
        /// </summary>
        /// <returns></returns>
        public async Task<uint> GetBaudRateAsync()
        {
            var response = await ExecuteAtQueryAsync<BaudRateResponseData>(new BaudRateCommand());
            return response.BaudRate;
        }

        /// <summary>
        /// Set the baud rate for the serial interface on this node.
        /// </summary>
        /// <param name="baudRate"></param>
        /// <returns></returns>
        public async Task SetBaudRateAsync(BaudRate baudRate)
        {
            await ExecuteAtCommandAsync(new BaudRateCommand(baudRate), true);
        }

        /// <summary>
        /// Set a non-standard baud rate for the serial interface on this node.
        /// </summary>
        /// <param name="baudRate"></param>
        /// <returns></returns>
        public async Task SetBaudRateAsync(int baudRate)
        {
            await ExecuteAtCommandAsync(new BaudRateCommand(baudRate), true);
        }

        /// <summary>
        /// Get the configured API mode for this node.
        /// </summary>
        /// <returns></returns>
        public async Task<ApiMode> GetApiModeAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<ApiMode>>(new ApiEnableCommand());
            return response.Value;
        }

        /// <summary>
        /// Set the configured API mode for this node.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public async Task SetApiModeAsync(ApiMode mode)
        {
            await ExecuteAtCommandAsync(new ApiEnableCommand(mode));
        }

        /// <summary>
        /// Used to explicitly exit command mode.
        /// </summary>
        /// <returns></returns>
        public async Task ExitCommandModeAsync()
        {
            await ExecuteAtCommandAsync(new ExitCommandModeCommand());
        }

        /// <summary>
        /// Gets the static serial number of this node.
        /// </summary>
        /// <returns>The serial number</returns>
        public async Task<LongAddress> GetSerialNumberAsync()
        {
            PrimitiveResponseData<uint> highAddress =
                await ExecuteAtQueryAsync<PrimitiveResponseData<uint>>(new SerialNumberHighCommand());
            PrimitiveResponseData<uint> lowAddress =
                await ExecuteAtQueryAsync<PrimitiveResponseData<uint>>(new SerialNumberLowCommand());

            return new LongAddress(highAddress.Value, lowAddress.Value);
        }

        /// <summary>
        /// Commit configuration changes to this node.
        /// </summary>
        /// <returns></returns>
        public async Task WriteChangesAsync()
        {
            await ExecuteAtCommandAsync(new WriteCommand());
        }

        internal async Task<TResponseData> ExecuteAtQueryAsync<TResponseData>(AtCommand command)
            where TResponseData : AtCommandResponseFrameData
        {
            return await Controller.ExecuteAtQueryAsync<TResponseData>(command, Address);
        }

        internal virtual async Task ExecuteAtCommandAsync(AtCommand command, bool queueLocal = false)
        {
            await Controller.ExecuteAtCommandAsync(command, Address, queueLocal);
        }
    }
}