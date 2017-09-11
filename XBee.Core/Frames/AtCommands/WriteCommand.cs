namespace XBee.Frames.AtCommands
{
    internal class WriteCommand : AtCommand
    {
        public const string Name = "WR";

        public WriteCommand() : base(Name)
        {
        }
    }
}
