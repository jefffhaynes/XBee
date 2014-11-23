using System;

namespace XBee.Frames
{
    public class AtCommandException : Exception
    {
        public AtCommandStatus CommandStatus { get; private set; }

        public AtCommandException(AtCommandStatus commandStatus) : 
            base(string.Format("AT command failed with status '{0}'", commandStatus))
        {
            CommandStatus = commandStatus;
        }

        public AtCommandException(AtCommandStatus commandStatus, string message) : base(message)
        {
            CommandStatus = commandStatus;
        }
    }
}
