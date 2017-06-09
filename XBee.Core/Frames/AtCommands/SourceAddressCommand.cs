using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class SourceAddressCommand : AtCommand
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
            get => Parameter as ShortAddress;
            set => Parameter = value;
        }
    }
}
