using System;
using BinarySerialization;

namespace XBee
{
    public class ShortAddress : IEquatable<ShortAddress>
    {
        public static readonly ShortAddress Broadcast = new ShortAddress(0xffff);
        public static readonly ShortAddress Disabled = new ShortAddress(0xfffe);

        public ShortAddress()
        {
            Value = Disabled.Value;
        }

        public ShortAddress(ushort value)
        {
            Value = value;
        }

        public ushort Value { get; set; }

        public bool Equals(ShortAddress other)
        {
            return Value.Equals(other.Value);
        }

        public override string ToString()
        {
            return Value.ToString("X4");
        }

        [Ignore]
        public bool IsBroadcast => Value == Broadcast.Value;

        [Ignore]
        public bool IsDisabled => Value == Disabled.Value;
    }
}