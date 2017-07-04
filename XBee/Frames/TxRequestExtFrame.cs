using BinarySerialization;

namespace XBee.Frames
{
    internal class TxRequestExtFrame : CommandFrameContent
    {
        public TxRequestExtFrame()
        {
        }

        public TxRequestExtFrame(LongAddress destination, ShortAddress destinationShort, byte[] data)
        {
            Destination = destination;
            DestinationShort = destinationShort;
            Data = data;
        }

        [FieldOrder(0)]
        public LongAddress Destination { get; set; }

        [FieldOrder(1)]
        public ShortAddress DestinationShort { get; set; }

        [FieldOrder(2)]
        public byte BroadcastRadius { get; set; }

        [FieldOrder(3)]
        public TransmitOptionsExt Options { get; set; }

        [FieldOrder(4)]
        public byte[] Data { get; set; }
    }
}
