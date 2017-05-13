namespace XBee.Frames.AtCommands
{
    public class CellularDeviceOptionCommand : AtCommand
    {
        public CellularDeviceOptionCommand() : base("DO")
        {
        }

        public CellularDeviceOptionCommand(CellularDeviceOption option)
        {
            Parameter = option;
        }
    }
}
