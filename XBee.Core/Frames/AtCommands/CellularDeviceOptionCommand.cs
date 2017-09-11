namespace XBee.Frames.AtCommands
{
    internal class CellularDeviceOptionCommand : AtCommand
    {
        public const string Name = "DO";

        public CellularDeviceOptionCommand() : base(Name)
        {
        }

        public CellularDeviceOptionCommand(CellularDeviceOption option) : this()
        {
            Parameter = option;
        }
    }
}
