using BinarySerialization;

namespace XBee.Frames
{
    public class RxIndicatorExtFrame : CommandFrameContent
    {
        private static readonly byte[] ReservedData = { 0xff, 0xfe };

        public RxIndicatorExtFrame()
        {
            Reserved = ReservedData;
        }

        public LongAddress Source { get; set; }

        [FieldLength(2)]
        public byte[] Reserved { get; set; }

        public ReceiveOptionsExt Options { get; set; }

        public byte[] Data { get; set; }
    }
}
