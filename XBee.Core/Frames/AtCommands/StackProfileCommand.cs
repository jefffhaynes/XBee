namespace XBee.Frames.AtCommands
{
    internal class StackProfileCommand : AtCommand
    {
        public const string Name = "ZS";

        public StackProfileCommand() : base(Name)
        {
        }

        public StackProfileCommand(byte value) : this()
        {
            Parameter = value;
        }
    }
}
