using BinarySerialization;
using XBee.Frames.AtCommands;

namespace XBee.Frames
{
    internal class AtCommandResponseFrameContent
    {
        private const int AtCommandFieldLength = 2;

        [FieldOrder(0)]
        [FieldLength(AtCommandFieldLength)]
        public string AtCommand { get; set; }

        [FieldOrder(1)]
        public AtCommandStatus Status { get; set; }

        [FieldOrder(2)]
        [Subtype(nameof(AtCommand), ApiEnableCommand.Name, typeof(PrimitiveResponseData<ApiMode>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), AssociationIndicationCommand.Name, typeof(PrimitiveResponseData<AssociationIndicator>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), EndDeviceAssociationOptionsCommand.Name, typeof(PrimitiveResponseData<EndDeviceAssociationOptions>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), CoordinatorAssociationOptionsCommand.Name, typeof(PrimitiveResponseData<CoordinatorAssociationOptions>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), BaudRateCommand.Name, typeof(BaudRateResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), ParityCommand.Name, typeof(PrimitiveResponseData<Parity>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), StopBitsCommand.Name, typeof(PrimitiveResponseData<StopBits>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), PacketizationTimeoutCommand.Name, typeof(PrimitiveResponseData<byte>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), FlowControlThresholdCommand.Name, typeof(PrimitiveResponseData<byte>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), OperatingChannelCommand.Name, typeof(PrimitiveResponseData<byte>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), ScanChannelsCommand.Name, typeof(PrimitiveResponseData<ScanChannels>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), ScanDurationCommand.Name, typeof(PrimitiveResponseData<byte>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), NodeJoinTimeCommand.Name, typeof(PrimitiveResponseData<byte>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), ChannelVerificationCommand.Name, typeof(PrimitiveResponseData<bool>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), NetworkWatchdogTimeoutCommand.Name, typeof(PrimitiveResponseData<ushort>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), SourceAddressCommand.Name, typeof(PrimitiveResponseData<byte[]>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), StackProfileCommand.Name, typeof(PrimitiveResponseData<byte>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), DestinationAddressHighCommand.Name, typeof(PrimitiveResponseData<uint>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), DestinationAddressLowCommand.Name, typeof(PrimitiveResponseData<uint>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), PanIdCommand.Name, typeof(PanIdResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), SleepModeCommand.Name, typeof(PrimitiveResponseData<SleepMode>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), SleepOptionsCommand.Name, typeof(SleepOptionsResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), SleepPeriodCommand.Name, typeof(SleepPeriodResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), PullUpResistorConfigurationCommand.Name, typeof(PullUpResistorConfigurationResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), RssiPwmTimeCommand.Name, typeof(RssiPwmTimeResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), SleepPeriodCountCommand.Name, typeof(PrimitiveResponseData<ushort>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), NetworkDiscoveryCommand.Name, typeof(NetworkDiscoveryResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), NetworkDiscoveryTimeoutCommand.Name, typeof(NetworkDiscoveryTimeoutResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), HardwareVersionCommand.Name, typeof(HardwareVersionResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), FirmwareVersionCommand.Name, typeof(PrimitiveResponseData<ushort>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), CoordinatorEnableCommand.Name, typeof(CoordinatorEnableResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), MaximumPayloadBytesCommand.Name, typeof(PrimitiveResponseData<ushort>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), NodeIdentifierCommand.Name, typeof(NodeIdentifierResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), SerialNumberHighCommand.Name, typeof(PrimitiveResponseData<uint>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), SerialNumberLowCommand.Name, typeof(PrimitiveResponseData<uint>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), SampleRateCommand.Name, typeof(SampleRateResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), InputOutputChangeDetectionCommand.Name, typeof(InputOutputChangeDetectionResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), EncryptionEnableCommand.Name, typeof(PrimitiveResponseData<bool>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), PhoneNumberCommand.Name, typeof(StringResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), IccidCommand.Name, typeof(StringResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), ImeiCommand.Name, typeof(StringResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), NetworkOperatorCommand.Name, typeof(StringResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), ModemFirmwareVersionCommand.Name, typeof(StringResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), CellularSignalStrengthCommand.Name, typeof(PrimitiveResponseData<byte>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), InternetProtocolCommand.Name, typeof(PrimitiveResponseData<InternetProtocol>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), SslProtocolCommand.Name, typeof(PrimitiveResponseData<SslProtocol>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), TcpClientConnectionTimeoutCommand.Name, typeof(TcpClientConnectionTimeoutResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), CellularDeviceOptionCommand.Name, typeof(PrimitiveResponseData<CellularDeviceOption>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), AccessPointNameCommand.Name, typeof(StringResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), PowerLevelCommand.Name, typeof(PrimitiveResponseData<byte>), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), "D0", typeof(InputOutputResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), "D1", typeof(InputOutputResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), "D2", typeof(InputOutputResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), "D3", typeof(InputOutputResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), "D4", typeof(InputOutputResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), "D5", typeof(InputOutputResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), "D6", typeof(InputOutputResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), "D7", typeof(InputOutputResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), "D8", typeof(InputOutputResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype(nameof(AtCommand), "D9", typeof(InputOutputResponseData), BindingMode = BindingMode.OneWay)]
        public AtCommandResponseFrameData Data { get; set; }
    }
}
