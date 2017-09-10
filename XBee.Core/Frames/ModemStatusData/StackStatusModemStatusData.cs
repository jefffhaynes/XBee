using BinarySerialization;

namespace XBee.Frames.ModemStatusData
{
    internal class StackStatusModemStatusData : ModemStatusExtData
    {
        [FieldOrder(0)]
        public EmberStatus EmberStatus { get; set; }

        [FieldOrder(1)]
        public EmberNetworkState EmberNetworkState { get; set; }
    }
}
