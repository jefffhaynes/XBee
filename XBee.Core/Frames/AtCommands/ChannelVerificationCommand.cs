namespace XBee.Frames.AtCommands
{
    internal class ChannelVerificationCommand : AtCommand
    {
        public const string Name = "JV";

        public ChannelVerificationCommand() : base(Name)
        {
        }

        public ChannelVerificationCommand(bool verify) : this()
        {
            Parameter = verify;
        }
    }
}
