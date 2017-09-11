using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class DestinationAddressHighCommand : AtCommand
    {
        public const string Name = "DH";

        public DestinationAddressHighCommand() : base(Name)
        {
        }

        public DestinationAddressHighCommand(uint high) : this()
        {
            High = high;
        }

        [Ignore]
        public uint? High
        {
            get => Parameter as uint?;
            set => Parameter = value;
        }
    }
}
