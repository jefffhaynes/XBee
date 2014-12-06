namespace XBee.Frames
{
    public class RxIndicatorExplicitExtFrame : CommandFrameContent, IRxIndicatorDataFrame
    {
        public LongAddress Source { get; set; }

        public ShortAddress ShortAddress { get; set; }

        public byte SourceEndpoint { get; set; }

        public byte DestinationEndpoint { get; set; }

        public ushort ClusterId { get; set; }

        public ushort ProfileId { get; set; }

        public ReceiveOptionsExt Options { get; set; }

        public NodeAddress GetAddress()
        {
            return new NodeAddress(Source, ShortAddress);
        }

        public byte[] Data { get; set; }
    }
}
