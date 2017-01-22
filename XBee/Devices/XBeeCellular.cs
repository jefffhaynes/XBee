using System;
using System.Threading;
using System.Threading.Tasks;
using XBee.Frames;
using XBee.Frames.AtCommands;

namespace XBee.Devices
{
    public class XBeeCellular : XBeeNode, IAssociationIndicator
    {
        private const string NotSupportedMessage = "This feature is not supported on the XBee Cellular device.";

        public XBeeCellular(XBeeController controller, HardwareVersion hardwareVersion, NodeAddress address = null) : base(controller, hardwareVersion, address)
        {
        }

        /// <summary>
        ///     Occurs when an SMS message is received.
        /// </summary>
        public event EventHandler<SmsReceivedEventArgs> SmsReceived
        {
            add {  Controller.SmsReceived += value; }
            remove { Controller.SmsReceived -= value; }
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
        /// Gets the cellular signal strength for the modem.
        /// </summary>
        /// <returns>0x71 - 0x33 (-113 dBm to -51 dBm)</returns>
        public async Task<byte> GetCellularSignalStrengthAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<byte>>(new CellularSignalStrengthCommand());
            return response.Value;
        }

        /// <summary>
        /// Gets the configured Internet protocol for the device.
        /// </summary>
        /// <returns></returns>
        public async Task<InternetProtocol> GetInternetProtocolAsync()
        {
            var response = await ExecuteAtQueryAsync<PrimitiveResponseData<InternetProtocol>>(new InternetProtocolCommand());
            return response.Value;
        }

        /// <summary>
        /// Sets the Internet protocol for the device.
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

        /// <summary>
        /// Gets the Access Point Name (APN) the device uses to connect.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAccessPointNameAsync()
        {
            var response = await ExecuteAtQueryAsync<StringResponseData>(new AccessPointNameCommand());
            return response.Value;
        }

        /// <summary>
        /// Sets the Access Point Name (APN) the device uses to connect. The APN must match a valid value for the carrier.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task SetAccessPointNameAsync(string name)
        {
            await ExecuteAtCommandAsync(new AccessPointNameCommand(name));
        }
        
        /// <summary>
        /// Gets the network association state for this node.
        /// </summary>
        public async Task<AssociationIndicator> GetAssociationAsync()
        {
            PrimitiveResponseData<AssociationIndicator> response = await
                    Controller.ExecuteAtQueryAsync<PrimitiveResponseData<AssociationIndicator>>(
                        new AssociationIndicationCommand());

            return response.Value;
        }

        /// <summary>
        /// Send an SMS.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendSms(string phoneNumber, string message)
        {
            var cleanNumber = phoneNumber.Replace("-", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);
            var txSms = new TxSmsFrame(cleanNumber, message);
            await Controller.ExecuteAsync(txSms);
        }

        protected override NodeAddress GetAddressInternal()
        {
            return null;
        }

        #region Not Supported

        /// <summary>
        /// Not supported on XBee Cellular.
        /// </summary>
        public override NodeAddress Address
        {
            get
            {
                throw new NotSupportedException(NotSupportedMessage);
            }
        }

        public override Task<NodeAddress> GetAddressAsync()
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        public override Task SetDestinationAddressAsync(LongAddress address)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        public override Task SetSourceAddressAsync(ShortAddress address)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Not supported on XBee Cellular.
        /// </summary>
        public override Task ForceSampleAsync()
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Not supported on XBee Cellular.
        /// </summary>
        public override Task<byte> GetChannelAsync()
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Not supported on XBee Cellular.
        /// </summary>
        public override Task SetChannelAsync(byte channel)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Not supported on XBee Cellular.
        /// </summary>
        public override Task<LongAddress> GetSerialNumberAsync()
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Not supported on XBee Cellular.
        /// </summary>
        public override Task<DigitalSampleChannels> GetChangeDetectionChannelsAsync()
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Not supported on XBee Cellular.
        /// </summary>
        public override Task SetChangeDetectionChannelsAsync(DigitalSampleChannels channels)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Not supported on XBee Cellular.
        /// </summary>
        public override Task<SleepMode> GetSleepModeAsync()
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Not supported on XBee Cellular.
        /// </summary>
        public override Task SetSleepModeAsync(SleepMode mode)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }
        
        /// <summary>
        /// Not supported on XBee Cellular.
        /// </summary>
        public override Task<TimeSpan> GetSampleRateAsync()
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Not supported on XBee Cellular.
        /// </summary>
        public override Task SetSampleRateAsync(TimeSpan interval)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Not supported on XBee Cellular.
        /// </summary>
        public override Task<bool> IsEncryptionEnabledAsync()
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Not supported on XBee Cellular.
        /// </summary>
        public override Task SetEncryptionKeyAsync(byte[] key)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Not supported on XBee Cellular.
        /// </summary>
        public override Task<InputOutputConfiguration> GetInputOutputConfigurationAsync(InputOutputChannel channel)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }
        
        /// <summary>
        /// Not supported on XBee Cellular.
        /// </summary>
        public override Task SetInputOutputConfigurationAsync(InputOutputChannel channel, InputOutputConfiguration configuration)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Not supported on XBee Cellular.
        /// </summary>
        public override XBeeStream GetSerialStream()
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Not supported on XBee Cellular.
        /// </summary>
        public override IObservable<byte[]> GetReceivedData()
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Not supported on XBee Cellular.
        /// </summary>
        public override IObservable<Sample> GetSamples()
        {
            throw new NotSupportedException(NotSupportedMessage);
        }
        
        #endregion
    }
}
