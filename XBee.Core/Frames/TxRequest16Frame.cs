using BinarySerialization;

namespace XBee.Frames
{
    public class TxRequest16Frame : CommandFrameContent
    {
        public TxRequest16Frame()
        {
        }


        public TxRequest16Frame(ShortAddress destination, byte[] data)
        {
            Destination = destination;
            Data = data;
        }

        [FieldOrder(0)]
        public ShortAddress Destination { get; set; }

        [FieldOrder(1)]
        public TransmitOptions Options { get; set; }

        [FieldOrder(2)]
        public byte[] Data { get; set; }
    }
}
