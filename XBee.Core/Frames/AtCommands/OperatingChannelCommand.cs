namespace XBee.Frames.AtCommands
{
    internal class OperatingChannelCommand : AtCommand
    {
        public const string Name = "CH";

        public OperatingChannelCommand() : base(Name)
        {
        }

        public OperatingChannelCommand(byte channel) : this()
        {
            Parameter = channel;
        }
    }
}
