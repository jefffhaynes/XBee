using BinarySerialization;
using JetBrains.Annotations;

namespace XBee.Frames
{
    internal class RxIndicatorExplicitExtFrame : FrameContent, IRxIndicatorDataFrame
    {
        [FieldOrder(0)]
        public LongAddress Source { get; set; }

        [FieldOrder(1)]
        public ShortAddress ShortAddress { get; set; }

        [FieldOrder(2)] [UsedImplicitly] public byte SourceEndpoint { get; set; }

        [FieldOrder(3)] [UsedImplicitly] public byte DestinationEndpoint { get; set; }

        [FieldOrder(4)] [UsedImplicitly] public ushort ClusterId { get; set; }

        [FieldOrder(5)] [UsedImplicitly] public ushort ProfileId { get; set; }

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
