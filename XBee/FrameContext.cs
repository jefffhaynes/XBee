using XBee.Frames.AtCommands;

namespace XBee
{
    public class FrameContext
    {
        public FrameContext(HardwareVersion? controllerHardwareVersion)
        {
            ControllerHardwareVersion = controllerHardwareVersion;
        }

        public HardwareVersion? ControllerHardwareVersion { get; set; }
    }
}
