namespace XBee.Frames.AtCommands
{
    public class InternetProtocolCommand : AtCommand
    {
        public InternetProtocolCommand() : base("IP")
        {
        }

        public InternetProtocolCommand(InternetProtocol protocol)
        {
            Parameter = protocol;
        }
    }
}
