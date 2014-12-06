namespace XBee
{
    public class ShortAddress
    {
        public static readonly ShortAddress Broadcast = new ShortAddress(0xffff);
        public static readonly ShortAddress Disabled = new ShortAddress(0xfffe);

        public ShortAddress()
        {
        }

        public ShortAddress(ushort value)
        {
            Value = value;
        }

        public ushort Value { get; set; }

        public override string ToString()
        {
            return Value.ToString("X4");
        }
    }
}