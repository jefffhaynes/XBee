using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class DestinationAddressLowCommand : AtCommand
    {
        public DestinationAddressLowCommand() : base("DL")
        {
        }

        public DestinationAddressLowCommand(uint low) : this()
        {
            Low = low;
        }

        [Ignore]
        public uint? Low
        {
            get { return Parameter as uint?; }
            set { Parameter = value; }
        }
    }
}
