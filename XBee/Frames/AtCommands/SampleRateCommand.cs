using System;
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class SampleRateCommand : AtCommandFrame
    {
        public SampleRateCommand() : base("IR")
        {
        }

        [Ignore]
        public TimeSpan Interval
        {
            get
            {
                if (Parameter == null)
                    return TimeSpan.Zero;

                var milliseconds = (ushort) Parameter;
                return TimeSpan.FromMilliseconds(milliseconds);
            }

            set
            {
                Parameter = value.TotalMilliseconds;
            }
        }
    }
}
