using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class SourceAddressCommand : AtCommand
    {
        public const string Name = "MY";

        public SourceAddressCommand() : base(Name)
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
