namespace XBee.Frames.AtCommands
{
    internal class RestoreDefaultsCommand : AtCommand
    {
        public const string Name = "RE";

        public RestoreDefaultsCommand() : base(Name)
        {
        }
    }
}
