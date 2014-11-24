using System;
using BinarySerialization;

namespace XBee.Frames
{
    public class AtRemoteCommandFrameContent : CommandFrameContent
    {
        private const int AtCommandFieldLength = 2;

        public AtRemoteCommandFrameContent() : this(string.Empty)
        {

        }

        public AtRemoteCommandFrameContent(string atCommand)
        {
            if(atCommand == null)
                throw new ArgumentNullException("atCommand");

            if(atCommand.Length > AtCommandFieldLength)
                throw new ArgumentException("Command cannot exceed field length.", "atCommand");

            AtCommand = atCommand;

            LegacyDestination = ShortAddress.Broadcast;
        }

        public LongAddress Destination { get; set; }

        [Obsolete]
        public ShortAddress ShortAddress { get; set; }

        [FieldLength(AtCommandFieldLength)]
        public string AtCommand { get; set; }

        public object Parameter { get; set; }
    }
}
