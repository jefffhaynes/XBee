using System;
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class SampleRateCommand : AtCommand
    {
        public SampleRateCommand() : base("IR")
        {
        }

        public SampleRateCommand(TimeSpan interval) : this()
        {
            Interval = interval;
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
                var interval = value.TotalMilliseconds;

                if(interval > ushort.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(value), interval, "Must be less than 0xFFFF");

                Parameter = (ushort)interval;
            }
        }
    }
}
