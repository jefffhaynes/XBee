namespace XBee.Frames.AtCommands
{
    internal class PacketizationTimeoutCommand : AtCommand
    {
        public const string Name = "RO";

        public PacketizationTimeoutCommand() : base(Name)
        {
        }

        public PacketizationTimeoutCommand(byte characterTimes) : this()
        {
            Parameter = characterTimes;
        }
    }
}
