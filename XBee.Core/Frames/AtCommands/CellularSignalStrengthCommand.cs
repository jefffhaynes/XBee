namespace XBee.Frames.AtCommands
{
    internal class CellularSignalStrengthCommand : AtCommand
    {
        public const string Name = "DB";

        public CellularSignalStrengthCommand() : base(Name)
        {
        }
    }
}
