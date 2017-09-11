namespace XBee.Frames.AtCommands
{
    internal class StopBitsCommand : AtCommand
    {
        public const string Name = "SB";

        public StopBitsCommand() : base(Name)
        {
        }

        public StopBitsCommand(StopBits stopBits) : this()
        {
            Parameter = stopBits;
        }
    }
}
