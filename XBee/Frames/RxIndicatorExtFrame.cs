namespace XBee.Frames
{
    public class RxIndicatorExtFrame : CommandFrameContent
    {
        public LongAddress Source { get; set; }

        public ShortAddress ShortAddress { get; set; }

        public ReceiveOptionsExt Options { get; set; }

        public byte[] Data { get; set; }
    }
}
