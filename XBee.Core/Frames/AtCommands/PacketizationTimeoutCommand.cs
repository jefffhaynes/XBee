namespace XBee.Frames.AtCommands
{
    public class PacketizationTimeoutCommand : AtCommand
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
