using BinarySerialization;

namespace XBee.Frames.ModemStatusData
{
    public class JoiningModemStatusData
    {
        [FieldOrder(0)]
        public byte RadioChannel { get; set; }

        [FieldOrder(1)]
        public byte RadioTxPower { get; set; }

        [FieldOrder(2)]
        public ushort PanId { get; set; }

        [FieldOrder(3)]
        public ulong ExtendedPanId { get; set; }
    }
}
