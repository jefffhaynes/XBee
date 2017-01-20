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
            var response = await ExecuteAtQueryAsync<StringResponseData>(new PhoneNumberCommand());
            return response.Value;
        }

        /// <summary>
        /// Gets the Integrated Circuit Card Identifier (ICCID) of the inserted SIM.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetIccidAsync()
        {
            var response = await ExecuteAtQueryAsync<StringResponseData>(new IccidCommand());
            return response.Value;
        }

        /// <summary>
        /// Gets the International Mobile Equipment Identity (IMEI) of this device.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetImeiAsync()
        {
            var response = await ExecuteAtQueryAsync<StringResponseData>(new ImeiCommand());
            return response.Value;
        }

        /// <summary>
        /// Gets the network operator on which the device is registered.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetNetworkOperatorAsync()
        {
            var response = await ExecuteAtQueryAsync<StringResponseData>(new NetworkOperatorCommand());
            return response.Value;
        }

        /// <summary>
        /// Gets the firmware version for the cellular modem.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetModemFirmwareVersionAsync()
        {
            var response = await ExecuteAtQueryAsync<StringResponseData>(new ModemFirmwareVersionCommand());
            return response.Value;
        }

        /// <summary>
        /// Gets the celluar signal strength for the modem.
        /// </summary>
        /// <returns>0x71 - 0x33 (-113 dBm to -51 dBm)</returns>
        public async Task<byte> GetCellularSignalStrengthAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<byte>>(new CellularSignalStrengthCommand());
            return response.Value;
        }

        /// <summary>
        /// Gets the configured internet protocol for the device.
        /// </summary>
        /// <returns></returns>
        public async Task<InternetProtocol> GetInternetProtocolAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<InternetProtocol>>(new InternetProtocolCommand());
            return response.Value;
        }

        /// <summary>
        /// Sets the internet protocol for the device.
        /// </summary>
        /// <param name="protocol"></param>
        /// <returns></returns>
        public async Task SetInternetProtocolAsync(InternetProtocol protocol)
        {
            await ExecuteAtCommandAsync(new InternetProtocolCommand(protocol));
        }

        /// <summary>
        /// Gets the configured SSL protocol for the device.
        /// </summary>
        /// <returns></returns>
        public async Task<SslProtocol> GetSslProtocolAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<SslProtocol>>(new SslProtocolCommand());
            return response.Value;
        }

        /// <summary>
        /// Sets the SSL protocol for the device.
        /// </summary>
        /// <param name="protocol"></param>
        /// <returns></returns>
        public async Task SetInternetProtocolAsync(SslProtocol protocol)
        {
            await ExecuteAtCommandAsync(new SslProtocolCommand(protocol));
        }

        /// <summary>
        /// Gets the configured TCP client connection timeout for the device.
        /// </summary>
        /// <returns></returns>
        public async Task<TimeSpan> GetTcpClientConnectionTimeoutAsync()
        {
            var response = await ExecuteAtQueryAsync<TcpClientConnectionTimeoutResponseData>(new TcpClientConnectionTimeoutCommand());
            return response.Timeout;
        }

        /// <summary>
        /// Sets the TCP client connection timeout for the device.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task SetTcpClientConnectionTimeoutAsync(TimeSpan timeout)
        {
            await ExecuteAtCommandAsync(new TcpClientConnectionTimeoutCommand(timeout));
        }

        /// <summary>
        /// Gets the configured device option for this device.
        /// </summary>
        /// <returns></returns>
        public async Task<CellularDeviceOption> GetDeviceOptionAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<CellularDeviceOption>>(new CellularDeviceOptionCommand());
            return response.Value;
        }

        /// <summary>
        /// Sets the device option for this device.
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public async Task SetDeviceOptionAsync(CellularDeviceOption option)
        {
            await ExecuteAtCommandAsync(new CellularDeviceOptionCommand(option));
        }
    }
}
