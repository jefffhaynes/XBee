namespace XBee.Frames
{
    internal class AtQueuedCommandFrameContent : AtCommandFrameContent
    {
        public AtQueuedCommandFrameContent()
        {
        }

        public AtQueuedCommandFrameContent(AtCommand command) : base(command)
        {
        }
    }
}
