using XBee.Frames.AtCommands;

namespace XBee.Utility.ViewModels
{
    public interface IHardware
    {
        HardwareVersion HardwareVersion { get; }
    }
}
