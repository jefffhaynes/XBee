using System;
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class RssiPwmTimeCommand : AtCommand
    {
        public const int TimeoutUnitMs = 100;

        public RssiPwmTimeCommand() : base("RP")
        {
        }

        public RssiPwmTimeCommand(TimeSpan timeout)
        {
            if (timeout < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), timeout, "must be greater than or equal to zero.");
            }

            var ms = timeout.TotalMilliseconds / TimeoutUnitMs;
            Parameter = (byte) Math.Min(ms, byte.MaxValue);
        }

        public RssiPwmTimeCommand(byte value)
        {
            Parameter = value;
        }

        [Ignore]
        public TimeSpan Timeout => TimeSpan.FromMilliseconds((byte)Parameter * TimeoutUnitMs);
    }
}
