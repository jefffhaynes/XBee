using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class InputOutputChangeDetectionResponseData : AtCommandResponseFrameData
    {
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeeSeries1, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeeProSeries1, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        [SerializeAs(SerializedType.UInt1)]
        public DigitalSampleChannels? Channels { get; set; }

        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeePro900, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeePro900HP, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        public DigitalSampleChannels? ChannelsExt { get; set; }
    }
}
