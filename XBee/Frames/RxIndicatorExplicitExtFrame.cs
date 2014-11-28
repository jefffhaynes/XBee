namespace XBee.Frames
{
    public class RxIndicatorExplicitExtFrame : CommandFrameContent
    {
        public LongAddress Source { get; set; }

        public ShortAddress ShortAddress { get; set; }

        public byte SourceEndpoint { get; set; }

        public byte DestinationEndpoint { get; set; }

        public ushort ClusterId { get; set; }

        public ushort ProfileId { get; set; }

        public ReceiveOptionsExt Options { get; set; }

        public byte[] Data { get; set; }
    }
}
