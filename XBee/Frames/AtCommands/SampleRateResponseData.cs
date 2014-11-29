using System;
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class SampleRateResponseData : AtCommandResponseFrameData
    {
        public ushort Value { get; set; }

        [Ignore]
        public TimeSpan Interval
        {
            get { return TimeSpan.FromMilliseconds(Value); }
        }
    }
}
