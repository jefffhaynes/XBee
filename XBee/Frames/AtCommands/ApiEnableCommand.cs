namespace XBee.Frames.AtCommands
{
    public class ApiEnableCommand : AtCommand
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
