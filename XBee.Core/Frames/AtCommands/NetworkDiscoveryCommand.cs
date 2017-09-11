namespace XBee.Frames.AtCommands
{
    internal class NetworkDiscoveryCommand : AtCommand
    {
        public const string Name = "ND";

        public NetworkDiscoveryCommand() : base(Name)
        {
        }
    }
}
