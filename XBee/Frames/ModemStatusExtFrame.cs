using BinarySerialization;
using XBee.Frames.ModemStatusData;

namespace XBee.Frames
{
    public class ModemStatusExtFrame : FrameContent
    {
        [FieldOrder(0)]
        public ModemStatusExt ModemStatus { get; set; }

        [FieldOrder(1)]
        public ModemStatusExtData StatusData { get; set; }
    }
}
