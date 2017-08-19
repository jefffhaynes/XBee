namespace XBee.Frames.AtCommands
{
    internal class ParityCommand : AtCommand
    {
        public ParityCommand() : base("NB")
        {
        }

        public ParityCommand(Parity parity) : this()
        {
            Parameter = parity;
        }
    }
}
