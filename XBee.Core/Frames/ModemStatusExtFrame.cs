using BinarySerialization;
using JetBrains.Annotations;
using XBee.Frames.ModemStatusData;

namespace XBee.Frames
{
    internal class ModemStatusExtFrame : FrameContent
    {
        [FieldOrder(0)] [UsedImplicitly] public ModemStatusExt ModemStatus { get; set; }

        [FieldOrder(1)]
        [Subtype("ModemStatus", ModemStatusExt.Rejoin, typeof(RejoinModemStatusData), BindingMode = BindingMode.OneWayToSource)]
        [Subtype("ModemStatus", ModemStatusExt.StackStatus, typeof(StackStatusModemStatusData), BindingMode = BindingMode.OneWayToSource)]
        [Subtype("ModemStatus", ModemStatusExt.Joining, typeof(JoiningModemStatusData), BindingMode = BindingMode.OneWayToSource)]
        [SubtypeDefault(typeof(UnsupportedStatusExtData))]
        [UsedImplicitly]
        public ModemStatusExtData StatusData { get; set; }
    }
}
