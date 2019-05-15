using BinarySerialization;
using JetBrains.Annotations;

namespace XBee.Frames
{
    internal class TxRequestExtFrame : CommandFrameContent
    {
        [UsedImplicitly]
        public TxRequestExtFrame()
        {
        }

        public TxRequestExtFrame(LongAddress destination, ShortAddress shortDestination, byte[] data)
        {
            Destination = destination;
            ShortDestination = shortDestination;
            Data = data;
        }

        [FieldOrder(0)]
        public LongAddress Destination { get; set; }

        [FieldOrder(1)]
        public ShortAddress ShortDestination { get;set; }

        [FieldOrder(2)]
        public byte BroadcastRadius { get; set; }

        [FieldOrder(3)]
        public TransmitOptionsExt Options { get; set; }

        [FieldOrder(4)]
        public byte[] Data { get; set; }
    }
}
