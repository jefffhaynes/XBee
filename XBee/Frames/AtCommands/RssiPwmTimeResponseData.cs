using System;
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class RssiPwmTimeResponseData : AtCommandResponseFrameData
    {
        public byte TimeoutValue { get; set; }

        [Ignore]
        public TimeSpan Timeout => TimeSpan.FromMilliseconds(TimeoutValue * RssiPwmTimeCommand.TimeoutUnitMs);
    }
}
