namespace XBee.Frames.AtCommands
{
    internal class ParityCommand : AtCommand
    {
        public ParityCommand() : base("NB")
        {
        }

        public ParityCommand(Parity parity)
        {
            Parameter = parity;
        }
    }
}
