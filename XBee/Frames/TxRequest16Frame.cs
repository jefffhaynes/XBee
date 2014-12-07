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

        public ShortAddress Destination { get; set; }

        public TransmitOptions Options { get; set; }

        public byte[] Data { get; set; }
    }
}
