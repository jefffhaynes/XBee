using BinarySerialization;
using XBee.Frames.AtCommands;

namespace XBee.Frames
{
    public class AtCommandResponseFrameContent
    {
        private const int AtCommandFieldLength = 2;

        [FieldOrder(0)]
        [FieldLength(AtCommandFieldLength)]
        public string AtCommand { get; set; }

        [FieldOrder(1)]
        public AtCommandStatus Status { get; set; }

        [FieldOrder(2)]
        [Subtype("AtCommand", "AI", typeof(PrimitiveResponseData<AssociationIndicator>), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "MY", typeof(PrimitiveResponseData<ShortAddress>), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "DH", typeof(PrimitiveResponseData<uint>), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "DL", typeof(PrimitiveResponseData<uint>), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "SM", typeof(PrimitiveResponseData<SleepMode>), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "SO", typeof(SleepOptionsResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "ND", typeof(NetworkDiscoveryResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "HV", typeof(HardwareVersionResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "CE", typeof(CoordinatorEnableResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "NI", typeof(NodeIdentifierResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "SH", typeof(PrimitiveResponseData<uint>), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "SL", typeof(PrimitiveResponseData<uint>), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "D0", typeof(InputOutputResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "D1", typeof(InputOutputResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "D2", typeof(InputOutputResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "D3", typeof(InputOutputResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "D4", typeof(InputOutputResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "D5", typeof(InputOutputResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "D6", typeof(InputOutputResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "D7", typeof(InputOutputResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "D8", typeof(InputOutputResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "D9", typeof(InputOutputResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "IR", typeof(SampleRateResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "IC", typeof(InputOutputChangeDetectionResponseData), BindingMode = BindingMode.OneWay)]
        [Subtype("AtCommand", "EE", typeof(PrimitiveResponseData<bool>), BindingMode = BindingMode.OneWay)]
        public AtCommandResponseFrameData Data { get; set; }
    }
}
