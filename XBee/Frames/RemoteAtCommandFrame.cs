using System;
using BinarySerialization;

namespace XBee.Frames
{
    public class RemoteAtCommandFrame : CommandFrameContent
    {
        private const int AtCommandFieldLength = 2;

        public RemoteAtCommandFrame() : this(string.Empty)
        {
        }

        public RemoteAtCommandFrame(string atCommand)
        {
            if(atCommand == null)
                throw new ArgumentNullException("atCommand");

            if(atCommand.Length > AtCommandFieldLength)
                throw new ArgumentException("Command cannot exceed field length.", "atCommand");

            AtCommand = atCommand;

            ShortAddress = ShortAddress.Broadcast;

            Options = RemoteCommandOptions.Commit;
        }

        public LongAddress Destination { get; set; }

        public ShortAddress ShortAddress { get; set; }

        public RemoteCommandOptions Options { get; set; }

        [FieldLength(AtCommandFieldLength)]
        public string AtCommand { get; set; }

        public object Parameter { get; set; }
    }
}
