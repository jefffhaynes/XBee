namespace XBee.Frames.AtCommands
{
    internal class ApiEnableCommand : AtCommand
    {
        public const string Name = "AP";

        public ApiEnableCommand() : base(Name)
        {
        }

        public ApiEnableCommand(ApiMode mode) : this()
        {
            Parameter = mode;
        }
    }
}
