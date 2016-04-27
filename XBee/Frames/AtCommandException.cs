using System;

namespace XBee.Frames
{
    public class AtCommandException : Exception
    {
        public AtCommandStatus CommandStatus { get; private set; }

        public AtCommandException(AtCommandStatus commandStatus) : 
            base($"AT command failed with status '{commandStatus}'")
        {
            CommandStatus = commandStatus;
        }

        public AtCommandException(AtCommandStatus commandStatus, string message) : base(message)
        {
            CommandStatus = commandStatus;
        }
    }
}
