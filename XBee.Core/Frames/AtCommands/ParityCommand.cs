namespace XBee.Frames.AtCommands
{
    public class ParityCommand : AtCommand
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
