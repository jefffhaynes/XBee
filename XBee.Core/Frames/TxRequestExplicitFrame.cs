using BinarySerialization;

namespace XBee.Frames
{
    public class TxRequestExplicitFrame : CommandFrameContent
    {
        public TxRequestExplicitFrame()
        {
        }


        public TxRequestExplicitFrame(LongAddress destination, byte[] data)
        {
            Destination = destination;
            Data = data;
        }

        [FieldOrder(0)]
        public LongAddress Destination { get; set; }

        [FieldOrder(1)]
        public ShortAddress ShortDestination { get; set; }

        [FieldOrder(2)]
        public byte SourceEndpoint { get; set; }

        [FieldOrder(3)]
        public byte DestinationEndpoint { get; set; }

        [FieldOrder(4)]
        public ushort ClusterId { get; set; }

        [FieldOrder(5)]
        public ushort ProfileId { get; set; }

        [FieldOrder(6)]
        public byte BroadcastRadius { get; set; }

        [FieldOrder(7)]
        public TransmitOptionExplicit Options { get; set; }

        [FieldOrder(8)]
        public byte[] Data { get; set; }
    }
}
