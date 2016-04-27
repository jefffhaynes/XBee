using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class NetworkDiscoveryResponseData : AtCommandResponseFrameData
    {
        [FieldOrder(0)]
        public ShortAddress ShortAddress { get; set; }

        [FieldOrder(1)]
        public LongAddress LongAddress { get; set; }

        [FieldOrder(2)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeeSeries1, RelativeSourceMode = RelativeSourceMode.SerializationContext)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeeProSeries1, RelativeSourceMode = RelativeSourceMode.SerializationContext)]
        public ReceivedSignalStrengthIndicator ReceivedSignalStrengthIndicator { get; set; }

        [FieldOrder(3)]
        public string Name { get; set; }

        [FieldOrder(4)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.ZNetZigBeeS2, RelativeSourceMode = RelativeSourceMode.SerializationContext)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeeProS2, RelativeSourceMode = RelativeSourceMode.SerializationContext)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeeProS2B, RelativeSourceMode = RelativeSourceMode.SerializationContext)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeeProS2C, RelativeSourceMode = RelativeSourceMode.SerializationContext)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBee24S2C, RelativeSourceMode = RelativeSourceMode.SerializationContext)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeePro900HP, RelativeSourceMode = RelativeSourceMode.SerializationContext)]
        public NetworkDiscoveryResponseDataExtendedInfo ExtendedInfo { get; set; }

        [Ignore]
        public bool IsCoordinator
        {
            get
            {
                if (LongAddress.IsCoordinator)
                    return true;

                return ExtendedInfo != null && ExtendedInfo.DeviceType == DeviceType.Coordinator;
            }
        }

        public override string ToString()
        {
            return $"{Name}: {LongAddress}";
        }
    }
}
