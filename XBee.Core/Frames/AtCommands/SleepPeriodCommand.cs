using System;
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class SleepPeriodCommand : AtCommand
    {
        public const string Name = "SP";

        public const int PeriodMultiplier = 10;
        private const int PeriodMinBase = 0x20;
        private const int PeriodMaxBase = 0xaf0;

        private static readonly TimeSpan PeriodMin = TimeSpan.FromMilliseconds(PeriodMinBase * PeriodMultiplier);
        private static readonly TimeSpan PeriodMax = TimeSpan.FromMilliseconds(PeriodMaxBase * PeriodMultiplier);

        public SleepPeriodCommand() : base(Name)
        {
        }

        public SleepPeriodCommand(TimeSpan period) : this()
        {
            if (period < PeriodMin)
            {
                throw new ArgumentOutOfRangeException(nameof(period), period,
                    $"Must be greater than {PeriodMin.TotalMilliseconds} milliseconds.");
            }

            if (period > PeriodMax)
            {
                throw new ArgumentOutOfRangeException(nameof(period), period,
                    $"Must be less than {PeriodMax.TotalMilliseconds} milliseconds.");
            }

            Period = period;
        }
        
        [Ignore]
        public TimeSpan? Period
        {
            get
            {
                if (Parameter == null)
                {
                    return null;
                }

                return TimeSpan.FromMilliseconds((ushort)Parameter * PeriodMultiplier);
            }

            set
            {
                if (value == null)
                {
                    Parameter = null;
                }
                else
                {
                    Parameter = (ushort)(value.Value.TotalMilliseconds / PeriodMultiplier);
                }
            }
        }
    }
}
