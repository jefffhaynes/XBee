namespace XBee.Frames
{
    public class RxIndicatorExtFrame : CommandFrameContent, IRxIndicatorDataFrame
    {
        public LongAddress Source { get; set; }

        public ShortAddress ShortAddress { get; set; }

        public ReceiveOptionsExt Options { get; set; }

        public NodeAddress GetAddress()
        {
            return new NodeAddress(Source, ShortAddress);
        }

        public byte[] Data { get; set; }
    }
}
