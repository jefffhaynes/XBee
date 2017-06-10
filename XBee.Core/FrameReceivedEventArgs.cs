using System;

namespace XBee
{
    internal class FrameReceivedEventArgs : EventArgs
    {
        public FrameReceivedEventArgs(FrameContent frameContent)
        {
            FrameContent = frameContent;
        }

        public FrameContent FrameContent { get; }
    }
}
