namespace XBee.Frames.AtCommands
{
    internal class ScanChannelsCommand : AtCommand
    {
        public ScanChannelsCommand() : base("SC")
        {
        }

        public ScanChannelsCommand(ScanChannels scanChannels) : this()
        {
            Parameter = scanChannels;
        }

        public ScanChannels GetScanChannels()
        {
            return (ScanChannels) Parameter;
        }
    }
}
