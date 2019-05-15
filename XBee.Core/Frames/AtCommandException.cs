using System;
using JetBrains.Annotations;

namespace XBee.Frames
{
    [PublicAPI]
    public class AtCommandException : Exception
    {
        public AtCommandStatus CommandStatus { get; }

        public AtCommandException(AtCommandStatus commandStatus) : 
            base($"AT command failed with status '{commandStatus}'")
        {
            CommandStatus = commandStatus;
        }
    }
}
