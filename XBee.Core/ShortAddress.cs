using System;
using BinarySerialization;

namespace XBee
{
    public class ShortAddress : IEquatable<ShortAddress>
    {
        public static readonly ShortAddress Broadcast = new ShortAddress(0xffff);
        public static readonly ShortAddress Disabled = new ShortAddress(0xfffe);
        public static readonly ShortAddress Coordinator = new ShortAddress(0);

        public ShortAddress()
        {
            Value = Disabled.Value;
        }

        public ShortAddress(ushort value)
        {
            Value = value;
        }

        public ushort Value { get; }

        [Ignore]
        public bool IsBroadcast => Value == Broadcast.Value;

        [Ignore]
        public bool IsDisabled => Value == Disabled.Value;

        [Ignore]
        public bool IsCoordinator => Value == Coordinator.Value;

        public bool Equals(ShortAddress other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Value == other.Value;
        }

        public override string ToString()
        {
            return Value.ToString("X4");
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
            return Equals((ShortAddress) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}