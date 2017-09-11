namespace XBee.Frames.AtCommands
{
    internal class JoinNotificationCommand : AtCommand
    {
        public const string Name = "JN";

        public JoinNotificationCommand() : base(Name)
        {
        }

        public JoinNotificationCommand(bool enable) : this()
        {
            Parameter = enable;
        }
    }
}
