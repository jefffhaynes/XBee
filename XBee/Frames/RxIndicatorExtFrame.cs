using BinarySerialization;

namespace XBee.Frames
{
    public class RxIndicatorExtFrame : FrameContent, IRxIndicatorDataFrame
    {
        [FieldOrder(0)]
        public LongAddress Source { get; set; }

        [FieldOrder(1)]
        public ShortAddress ShortAddress { get; set; }

        [FieldOrder(2)]
        public ReceiveOptionsExt Options { get; set; }

        public NodeAddress GetAddress()
        {
            return new NodeAddress(Source, ShortAddress);
        }

        [FieldOrder(3)]
        public byte[] Data { get; set; }
    }
}
