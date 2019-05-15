using JetBrains.Annotations;

namespace XBee.Frames
{
    internal class AtQueuedCommandFrameContent : AtCommandFrameContent
    {
        [UsedImplicitly]
        public AtQueuedCommandFrameContent()
        {
        }

        public AtQueuedCommandFrameContent(AtCommand command) : base(command)
        {
        }
    }
}
