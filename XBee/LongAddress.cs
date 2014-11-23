
using System;

namespace XBee
{
    public class LongAddress : IEquatable<LongAddress>
    {
        public static readonly LongAddress BroadcastAddress = new LongAddress(0xFFFF);
        public static readonly LongAddress CoordinatorAddress = new LongAddress(0);

        public LongAddress()
        {
        }

        public LongAddress(ulong value)
        {
            Value = value;
        }

        public ulong Value { get; set; }

        public bool Equals(LongAddress other)
        {
            return Value.Equals(other.Value);
        }

        public override string ToString()
        {
            return Value.ToString("X16");
        }
    }
}
