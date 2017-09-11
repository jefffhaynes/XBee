namespace XBee.Frames.AtCommands
{
    internal class PowerLevelCommand : AtCommand
    {
        public const string Name = "PL";

        public PowerLevelCommand() : base(Name)
        {
        }

        public PowerLevelCommand(byte powerLevel) : this()
        {
            Parameter = powerLevel;
        }
    }
}
