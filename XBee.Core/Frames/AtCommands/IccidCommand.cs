namespace XBee.Frames.AtCommands
{
    internal class IccidCommand : AtCommand
    {
        public const string Name = "S#";

        public IccidCommand() : base(Name)
        {
        }
    }
}
