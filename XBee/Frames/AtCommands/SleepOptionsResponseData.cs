using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class SleepOptionsResponseData : AtCommandResponseFrameData
    {
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeeSeries1, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeeProSeries1, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        public SleepOptions? Options { get; set; }

        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeePro900, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeePro900HP, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        public SleepOptionsExt? OptionsExt { get; set; }
    }
}
