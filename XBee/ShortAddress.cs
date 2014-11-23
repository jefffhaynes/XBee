
namespace XBee
{
    public class ShortAddress
    {
        private ushort _value;

        public ushort Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override string ToString()
        {
            return Value.ToString("X4");
        }
    }
}
