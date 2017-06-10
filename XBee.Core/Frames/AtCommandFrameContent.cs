namespace XBee.Frames
{
    internal class AtCommandFrameContent : CommandFrameContent
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
