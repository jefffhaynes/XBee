using BinarySerialization;

namespace XBee.Frames
{
    public class RxIndicatorExplicitExtFrame : CommandFrameContent
    {
                private static readonly byte[] ReservedData = { 0xff, 0xfe };

                public RxIndicatorExplicitExtFrame()
        {
            Reserved = ReservedData;
        }

        public LongAddress Source { get; set; }

        [FieldLength(2)]
        public byte[] Reserved { get; set; }

        public byte SourceEndpoint { get; set; }

        public byte DestinationEndpoint { get; set; }

        public ushort ClusterId { get; set; }

        public ushort ProfileId { get; set; }

        public ReceiveOptionsExt Options { get; set; }

        public byte[] Data { get; set; }
    }
}
