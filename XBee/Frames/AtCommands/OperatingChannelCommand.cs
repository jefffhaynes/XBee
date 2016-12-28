namespace XBee.Frames.AtCommands
{
    public class OperatingChannelCommand : AtCommand
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
