using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class NetworkDiscoveryResponseData : AtCommandResponseFrameData
    {
        public ShortAddress ShortAddress { get; set; }

        public LongAddress LongAddress { get; set; }

        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeeSeries1, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeeProSeries1, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        public ReceivedSignalStrengthIndicator ReceivedSignalStrengthIndicator { get; set; }

        public string Name { get; set; }

        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeeProS2, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeeProS2B, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeePro900HP, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        public NetworkDiscoveryResponseDataExtendedInfo ExtendedInfo { get; set; }

        [Ignore]
        public bool IsCoordinator
        {
            get
            {
                if (LongAddress.Equals(LongAddress.CoordinatorAddress))
                    return true;

                return ExtendedInfo != null && ExtendedInfo.DeviceType == DeviceType.Coordinator;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Name, LongAddress);
        }
    }
}
