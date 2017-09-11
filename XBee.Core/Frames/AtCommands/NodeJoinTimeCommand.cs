namespace XBee.Frames.AtCommands
{
    internal class NodeJoinTimeCommand : AtCommand
    {
        public const string Name = "NJ";

        public NodeJoinTimeCommand() : base(Name)
        {
        }

        public NodeJoinTimeCommand(byte value) : this()
        {
            Parameter = value;
        }
    }
}
