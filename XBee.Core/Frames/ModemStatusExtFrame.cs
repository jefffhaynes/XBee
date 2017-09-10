using BinarySerialization;
using XBee.Frames.ModemStatusData;

namespace XBee.Frames
{
    internal class ModemStatusExtFrame : FrameContent
    {
        [FieldOrder(0)]
        public ModemStatusExt ModemStatus { get; set; }

        [FieldOrder(1)]
        [Subtype("ModemStatus", ModemStatusExt.Rejoin, typeof(RejoinModemStatusData), BindingMode = BindingMode.OneWayToSource)]
        [Subtype("ModemStatus", ModemStatusExt.StackStatus, typeof(StackStatusModemStatusData), BindingMode = BindingMode.OneWayToSource)]
        [Subtype("ModemStatus", ModemStatusExt.Joining, typeof(JoiningModemStatusData), BindingMode = BindingMode.OneWayToSource)]
        [SubtypeDefault(typeof(UnsupportedStatusExtData))]
        public ModemStatusExtData StatusData { get; set; }
    }
}
