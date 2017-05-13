namespace XBee.Frames.AtCommands
{
    public class StopBitsCommand : AtCommand
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
