using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class DestinationAddressLowCommand : AtCommand
    {
        public const string Name = "DL";

        public DestinationAddressLowCommand() : base(Name)
        {
        }

        public DestinationAddressLowCommand(uint low) : this()
        {
            Low = low;
        }

        [Ignore]
        public uint? Low
        {
            get => Parameter as uint?;
            set => Parameter = value;
        }
    }
}
