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

        public LongAddress Destination { get; set; }

        public TransmitOptions Options { get; set; }

        public byte[] Data { get; set; }
    }
}
