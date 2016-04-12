
using System;
using BinarySerialization;

namespace XBee
{
    public class LongAddress : IEquatable<LongAddress>
    {
        public static readonly LongAddress Broadcast = new LongAddress(0xFFFF);
        public static readonly LongAddress CoordinatorAddress = new LongAddress(0);

        public LongAddress()
        {
        }

        public LongAddress(ulong value)
        {
            Value = value;
        }

        public LongAddress(uint high, uint low)
        {
            High = high;
            Low = low;
        }

        public ulong Value
        {
            get { return ((ulong)High << 32) + Low; }

            set
            {
                High = (uint)((value & 0xFFFFFFFF00000000UL) >> 32);
                Low = (uint)(value & 0x00000000FFFFFFFFUL);
            }
        }

        [Ignore]
        public uint High { get; set; }

        [Ignore]
        public uint Low { get; set; }

        public bool Equals(LongAddress other)
        {
            return Value.Equals(other.Value);
        }

        public override string ToString()
        {
            return Value.ToString("X16");
        }

        [Ignore]
        public bool IsBroadcast => Value == Broadcast.Value;

        [Ignore]
        public bool IsCoordinator => Value == CoordinatorAddress.Value;
    }
}
