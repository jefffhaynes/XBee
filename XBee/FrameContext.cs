using XBee.Frames.AtCommands;

namespace XBee
{
    public class FrameContext
    {
        public FrameContext(HardwareVersion? coordinatorHardwareVersion)
        {
            CoordinatorHardwareVersion = coordinatorHardwareVersion;
        }

        public HardwareVersion? CoordinatorHardwareVersion { get; private set; }
    }
}
