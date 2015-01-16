using BinarySerialization;

namespace XBee.Frames
{
    public class RxIndicatorExplicitExtFrame : CommandFrameContent, IRxIndicatorDataFrame
    {
        [FieldOrder(0)]
        public LongAddress Source { get; set; }

        [FieldOrder(1)]
        public ShortAddress ShortAddress { get; set; }

        [FieldOrder(2)]
        public byte SourceEndpoint { get; set; }

        [FieldOrder(3)]
        public byte DestinationEndpoint { get; set; }

        [FieldOrder(4)]
        public ushort ClusterId { get; set; }

        [FieldOrder(5)]
        public ushort ProfileId { get; set; }

        [FieldOrder(6)]
        public ReceiveOptionsExt Options { get; set; }

        public NodeAddress GetAddress()
        {
            return new NodeAddress(Source, ShortAddress);
        }

        [FieldOrder(7)]
        public byte[] Data { get; set; }
    }
}
