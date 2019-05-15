using JetBrains.Annotations;

namespace XBee.Frames
{
    internal class AtCommandFrameContent : CommandFrameContent
    {
        [UsedImplicitly]
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
