namespace XBee.Frames.AtCommands
{
    internal class PowerLevelCommand : AtCommand
    {
        public PowerLevelCommand() : base("PL")
        {
        }

        public PowerLevelCommand(byte powerLevel) : this()
        {
            Parameter = powerLevel;
        }
    }
}
