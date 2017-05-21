using XBee.Frames.AtCommands;

namespace XBee.Utility.ViewModels
{
    public class XBeeControllerViewModel : ViewModelBase
    {
        public XBeeControllerViewModel(Universal.XBeeController controller, HardwareVersion hardwareVersion)
        {
            Controller = controller;
            HardwareVersion = hardwareVersion;
        }

        public Universal.XBeeController Controller { get; }

        public HardwareVersion HardwareVersion { get; }
    }
}
