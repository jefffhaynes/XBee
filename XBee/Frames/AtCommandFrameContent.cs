namespace XBee.Frames
{
    public class AtCommandFrameContent : CommandFrameContent
    {
        public AtCommandFrameContent()
        {
        }

        public AtCommandFrameContent(AtCommand command)
        {
            Command = command;
        }

        public AtCommand Command { get; set; }
    }
}
