using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class CoordinatorEnableResponseData : AtCommandResponseFrameData
    {
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeeSeries1, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeeProSeries1, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        public CoordinatorEnableState? EnableState { get; set; }

        
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeePro900HP, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        public CoordinatorEnableStateExt? EnableStateExt { get; set; }
    }
}
