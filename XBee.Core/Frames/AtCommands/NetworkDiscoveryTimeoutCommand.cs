using System;

namespace XBee.Frames.AtCommands
{
    internal class NetworkDiscoveryTimeoutCommand : AtCommand
    {
        public const int ValueMsScale = 100;

        public static readonly TimeSpan TimeoutMin = TimeSpan.FromMilliseconds(0x20 * ValueMsScale);
        public static readonly TimeSpan TimeoutMax = TimeSpan.FromMilliseconds(0xFF * ValueMsScale);

        public NetworkDiscoveryTimeoutCommand() : base("NT")
        {
        }

        public NetworkDiscoveryTimeoutCommand(TimeSpan timeout) : this()
        {
            if (timeout < TimeoutMin)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), timeout, $"must be greater than {TimeoutMin.TotalSeconds} seconds.");
            }

            if (timeout > TimeoutMax)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), timeout, $"must be less than {TimeoutMax.TotalSeconds} seconds.");
            }

            var ms = timeout.TotalMilliseconds;
            var value = ms / ValueMsScale;
            
            Parameter = (byte)value;
        }
    }
}
