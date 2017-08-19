namespace XBee.Frames.AtCommands
{
    internal class CellularDeviceOptionCommand : AtCommand
    {
        public CellularDeviceOptionCommand() : base("DO")
        {
        }

        public CellularDeviceOptionCommand(CellularDeviceOption option) : this()
        {
            Parameter = option;
        }
    }
}
