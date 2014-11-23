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

        public LongAddress Destination { get; set; }

        [FieldLength(2)]
        public byte[] Reserved { get; set; }

        public byte BroadcastRadius { get; set; }

        public TransmitOptionsExt Options { get; set; }

        public byte[] Data { get; set; }
    }
}
