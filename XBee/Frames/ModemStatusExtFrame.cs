using BinarySerialization;
using XBee.Frames.ModemStatusData;

namespace XBee.Frames
{
    public class ModemStatusExtFrame : FrameContent
    {
        [FieldOrder(0)]
        public ModemStatusExt ModemStatus { get; set; }

        [FieldOrder(1)]
        [Subtype("ModemStatus", ModemStatusExt.Rejoin, typeof(RejoinModemStatusData))]
        [Subtype("ModemStatus", ModemStatusExt.StackStatus, typeof(StackStatusModemStatusData))]
        public ModemStatusExtData StatusData { get; set; }
    }
}
