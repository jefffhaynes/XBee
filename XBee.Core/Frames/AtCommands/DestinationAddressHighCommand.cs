using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class DestinationAddressHighCommand : AtCommand
    {
        public DestinationAddressHighCommand() : base("DH")
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
