using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class SourceAddressCommand : AtCommand
    {
        public SourceAddressCommand() : base("MY")
        {
        }

        public SourceAddressCommand(ShortAddress address) : this()
        {
            Address = address;
        }

        [Ignore]
        public ShortAddress Address
        {
            get { return Parameter as ShortAddress; }
            set { Parameter = value; }
        }
    }
}
