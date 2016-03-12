using BinarySerialization;

namespace XBee.Frames.ModemStatusData
{
    public class StackStatusModemStatusData
    {
        [FieldOrder(0)]
        public EmberStatus EmberStatus { get; set; }

        [FieldOrder(1)]
        public EmberNetworkState EmberNetworkState { get; set; }
    }
}
