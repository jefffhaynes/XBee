namespace XBee.Frames.AtCommands
{
    internal class ForceSampleCommand : AtCommand
    {
        public const string Name = "IS";

        public ForceSampleCommand() : base(Name)
        {
        }
    }
}
