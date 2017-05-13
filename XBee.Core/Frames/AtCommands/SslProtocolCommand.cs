namespace XBee.Frames.AtCommands
{
    public class SslProtocolCommand : AtCommand
    {
        public SslProtocolCommand() : base("TL")
        {
        }

        public SslProtocolCommand(SslProtocol protocol)
        {
            Parameter = protocol;
        }
    }
}
