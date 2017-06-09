namespace XBee.Frames.AtCommands
{
    internal class OperatingChannelCommand : AtCommand
    {
        public OperatingChannelCommand() : base("CH")
        {
        }

        public OperatingChannelCommand(byte channel) : this()
        {
            Parameter = channel;
        }
    }
}
