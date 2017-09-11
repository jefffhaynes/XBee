using System;
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class RssiPwmTimeCommand : AtCommand
    {
        public const string Name = "RP";

        public const int TimeoutUnitMs = 100;

        public RssiPwmTimeCommand() : base(Name)
        {
        }

        public RssiPwmTimeCommand(TimeSpan timeout) : this()
        {
            if (timeout < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), timeout, "must be greater than or equal to zero.");
            }

            var ms = timeout.TotalMilliseconds / TimeoutUnitMs;
            Parameter = (byte) Math.Min(ms, byte.MaxValue);
        }

        public RssiPwmTimeCommand(byte value) : this()
        {
            Parameter = value;
        }

        [Ignore]
        public TimeSpan Timeout => Parameter == null
            ? TimeSpan.Zero
            : TimeSpan.FromMilliseconds((byte) Parameter * TimeoutUnitMs);
    }
}
