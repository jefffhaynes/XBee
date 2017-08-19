namespace XBee.Frames.AtCommands
{
    internal class InternetProtocolCommand : AtCommand
    {
        public InternetProtocolCommand() : base("IP")
        {
        }

        public InternetProtocolCommand(InternetProtocol protocol) : this()
        {
            Parameter = protocol;
        }
    }
}
