namespace XBee.Frames.AtCommands
{
    internal class ApiEnableCommand : AtCommand
    {
        public ApiEnableCommand() : base("AP")
        {
        }

        public ApiEnableCommand(ApiMode mode) : this()
        {
            Parameter = mode;
        }
    }
}
