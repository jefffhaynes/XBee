using BinarySerialization;

namespace XBee.Frames
{
    public class TxRequestExtFrame : CommandFrameContent
    {
        private static readonly byte[] ReservedData = {0xff, 0xfe};

        public TxRequestExtFrame()
        {
            Reserved = ReservedData;
        }

        public TxRequestExtFrame(LongAddress destination, byte[] data) : this()
        {
            Destination = destination;
            Data = data;
        }

        [FieldOrder(0)]
        public LongAddress Destination { get; set; }

        [FieldOrder(1)]
        [FieldLength(2)]
        public byte[] Reserved { get; set; }

        [FieldOrder(2)]
        public byte BroadcastRadius { get; set; }

        [FieldOrder(3)]
        public TransmitOptionsExt Options { get; set; }

        [FieldOrder(4)]
        public byte[] Data { get; set; }
    }
}
