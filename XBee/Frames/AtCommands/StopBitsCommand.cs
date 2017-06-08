namespace XBee.Frames.AtCommands
{
    internal class StopBitsCommand : AtCommand
    {
        public StopBitsCommand() : base("SB")
        {
        }

        public StopBitsCommand(StopBits stopBits) : this()
        {
            Parameter = stopBits;
        }
    }
}
