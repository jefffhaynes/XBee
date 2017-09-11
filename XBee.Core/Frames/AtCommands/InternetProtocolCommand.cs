namespace XBee.Frames.AtCommands
{
    internal class InternetProtocolCommand : AtCommand
    {
        public const string Name = "IP";

        public InternetProtocolCommand() : base(Name)
        {
        }

        public InternetProtocolCommand(InternetProtocol protocol) : this()
        {
            Parameter = protocol;
        }
    }
}
