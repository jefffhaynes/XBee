namespace XBee.Frames.AtCommands
{
    internal class SslProtocolCommand : AtCommand
    {
        public const string Name = "TL";

        public SslProtocolCommand() : base(Name)
        {
        }

        public SslProtocolCommand(SslProtocol protocol) : this()
        {
            Parameter = protocol;
        }
    }
}
