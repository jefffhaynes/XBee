using BinarySerialization;

namespace XBee.Frames
{
    public class TxRequestFrame : CommandFrameContent
    {
        public TxRequestFrame()
        {
        }


        public TxRequestFrame(LongAddress destination, byte[] data)
        {
            Destination = destination;
            Data = data;
        }

        [FieldOrder(0)]
        public LongAddress Destination { get; set; }

        [FieldOrder(1)]
        public TransmitOptions Options { get; set; }

        [FieldOrder(2)]
        public byte[] Data { get; set; }
    }
}
