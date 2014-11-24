using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class CoordinatorEnableResponseData : AtCommandResponseFrameData
    {
        [SerializeWhen("CoordinatorHardwareVersion", HardwareVersion.XBeeSeries1, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        [SerializeWhen("CoordinatorHardwareVersion", HardwareVersion.XBeeProSeries1, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        public CoordinatorEnableState? EnableState { get; set; }

        
        [SerializeWhen("CoordinatorHardwareVersion", HardwareVersion.XBeePro900HP, AncestorType = typeof(FrameContext), Mode = RelativeSourceMode.FindAncestor)]
        public CoordinatorEnableStateExt? EnableStateExt { get; set; }
    }
}
