namespace XBee.Frames
{
    public class RemoteAtCommandResponseFrame : CommandResponseFrameContent
    {
        public LongAddress LongAddress { get; set; }

        public ShortAddress ShortAddress { get; set; }

        public AtCommandResponseFrameContent Content { get; set; }
    }
}
