namespace XBee.Frames.AtCommands
{
    internal class SupplyVoltageCommand : AtCommand
    {
        public const string Name = "%V";

        public SupplyVoltageCommand() : base(Name)
        {
        }
    }
}