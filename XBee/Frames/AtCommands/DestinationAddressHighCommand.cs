namespace XBee.Frames.AtCommands
{
    public class DestinationAddressHighCommand : AtCommand
    {
        public DestinationAddressHighCommand() : base("DH")
        {
        }

        public DestinationAddressHighCommand(uint high) : this()
        {
            High = high;
        }

        public uint? High
        {
            get { return Parameter as uint?; }
            set { Parameter = value; }
        }
    }
}
