namespace XBee.Frames
{
    public class RxIndicator16Frame : CommandFrameContent, IRxIndicatorDataFrame
    {
        public ShortAddress Source { get; set; }

        public ReceivedSignalStrengthIndicator ReceivedSignalStrengthIndicator { get; set; }

        public ReceiveOptions Options { get; set; }

        public NodeAddress GetAddress()
        {
            return new NodeAddress(Source);
        }

        public byte[] Data { get; set; }
    }
}
