using System;
using BinarySerialization;

namespace XBee.Frames
{
    public class AtCommandFrame : CommandFrameContent
    {
        private const int AtCommandFieldLength = 2;

        public AtCommandFrame() : this(string.Empty)
        {
        }

        public AtCommandFrame(string atCommand)
        {
            if(atCommand == null)
                throw new ArgumentNullException("atCommand");

            if(atCommand.Length > AtCommandFieldLength)
                throw new ArgumentException("Command cannot exceed field length.", "atCommand");

            AtCommand = atCommand;
        }

        [FieldLength(AtCommandFieldLength)]
        public string AtCommand { get; set; }

        public object Parameter { get; set; }
    }
}
