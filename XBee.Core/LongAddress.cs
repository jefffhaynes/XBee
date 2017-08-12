using System;
using BinarySerialization;

namespace XBee
{
    public class LongAddress : IEquatable<LongAddress>
    {
        public static readonly LongAddress Broadcast = new LongAddress(0xFFFF);
        public static readonly LongAddress Coordinator = new LongAddress(0);
        public static readonly LongAddress Disabled = new LongAddress(0xFFFFFFFFFFFFFFFF);

        [Obsolete("Use Coordinator.")]
        public static readonly LongAddress CoordinatorAddress = new LongAddress(0);

        public LongAddress()
        {
        }

        public LongAddress(ulong value)
        {
            High = (uint) ((value & 0xFFFFFFFF00000000UL) >> 32);
            Low = (uint) (value & 0x00000000FFFFFFFFUL);
        }

        public LongAddress(uint high, uint low)
        {
            High = high;
            Low = low;
        }

        public ulong Value => ((ulong) High << 32) + Low;

        [Ignore]
        public uint High { get; }

        [Ignore]
        public uint Low { get; }

        [Ignore]
        public bool IsBroadcast => Value == Broadcast.Value;

        [Ignore]
        public bool IsCoordinator => Value == Coordinator.Value;

        [Ignore]
        public bool IsDisabled => Value == Disabled.Value;

        public bool Equals(LongAddress other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return High == other.High && Low == other.Low;
        }

        public override string ToString()
        {
            return Value.ToString("X16");
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((LongAddress) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) High * 397) ^ (int) Low;
            }
        }
    }
}