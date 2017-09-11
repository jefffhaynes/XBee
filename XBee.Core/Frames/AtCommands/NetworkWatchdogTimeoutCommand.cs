namespace XBee.Frames.AtCommands
{
    internal class NetworkWatchdogTimeoutCommand : AtCommand
    {
        public const string Name = "NW";

        public NetworkWatchdogTimeoutCommand() : base(Name)
        {
        }

        public NetworkWatchdogTimeoutCommand(ushort minutes) : this()
        {
            Parameter = minutes;
        }
    }
}
