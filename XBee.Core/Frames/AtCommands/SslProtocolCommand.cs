namespace XBee.Frames.AtCommands
{
    internal class SslProtocolCommand : AtCommand
    {
        public SslProtocolCommand() : base("TL")
        {
        }

        public SslProtocolCommand(SslProtocol protocol) : this()
        {
            Parameter = protocol;
        }
    }
}
