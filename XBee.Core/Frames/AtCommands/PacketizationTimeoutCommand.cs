namespace XBee.Frames.AtCommands
{
    internal class PacketizationTimeoutCommand : AtCommand
    {
        public PacketizationTimeoutCommand() : base("RO")
        {
        }

        public PacketizationTimeoutCommand(byte characterTimes) : this()
        {
            Parameter = characterTimes;
        }
    }
}
