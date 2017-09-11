using System;

namespace XBee.Frames.AtCommands
{
    internal class TcpClientConnectionTimeoutCommand : AtCommand
    {
        public const string Name = "TM";

        public const int ValueMsScale = 100;

        public TcpClientConnectionTimeoutCommand() : base(Name)
        {
        }

        public TcpClientConnectionTimeoutCommand(TimeSpan timeout) : this()
        {
            if (timeout < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), timeout, "must be greater than zero.");
            }
            var ms = timeout.TotalMilliseconds;
            var value = ms / ValueMsScale;

            if (value > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), timeout, "must be less than 109 minutes.");
            }

            Parameter = (ushort) value;
        }
    }
}