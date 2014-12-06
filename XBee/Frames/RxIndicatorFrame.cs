namespace XBee.Frames
{
    public class RxIndicatorFrame : CommandFrameContent, IRxIndicatorDataFrame
    {
        public LongAddress Source { get; set; }

        public ReceivedSignalStrengthIndicator ReceivedSignalStrengthIndicator { get; set; }

        public ReceiveOptions Options { get; set; }

        public NodeAddress GetAddress()
        {
            return new NodeAddress(Source);
        }

        public byte[] Data { get; set; }
    }
}
