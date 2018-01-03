namespace XBee.Frames.AtCommands
{
    internal class PushCommissioningButtonCommand : AtCommand
    {
        public const string Name = "CB";

        public PushCommissioningButtonCommand() : base(Name)
        {
        }

        public PushCommissioningButtonCommand(byte count) : this()
        {
            Parameter = count;
        }
    }
}