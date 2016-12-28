namespace XBee.Frames
{
    public class AtQueuedCommandFrameContent : AtCommandFrameContent
    {
        public AtQueuedCommandFrameContent()
        {
        }

        public AtQueuedCommandFrameContent(AtCommand command) : base(command)
        {
        }
    }
}
