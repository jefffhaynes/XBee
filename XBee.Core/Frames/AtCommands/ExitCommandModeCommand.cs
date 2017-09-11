namespace XBee.Frames.AtCommands
{
    internal class ExitCommandModeCommand : AtCommand
    {
        public const string Name = "CN";

        public ExitCommandModeCommand() : base(Name)
        {
        }
    }
}
