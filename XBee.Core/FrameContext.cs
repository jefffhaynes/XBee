using BinarySerialization;
using XBee.Frames.AtCommands;

namespace XBee
{
    internal class FrameContext
    {
        [FieldOrder(0)]
        public HardwareVersion ControllerHardwareVersion { get; set; }

        [FieldOrder(1)]
        public XBeeProtocol? Protocol { get; set; }
    }
}
