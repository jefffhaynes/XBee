namespace XBee.Frames.AtCommands
{
    internal class ParityCommand : AtCommand
    {
        public const string Name = "NB";

        public ParityCommand() : base(Name)
        {
        }

        public ParityCommand(Parity parity) : this()
        {
            Parameter = parity;
        }
    }
}
