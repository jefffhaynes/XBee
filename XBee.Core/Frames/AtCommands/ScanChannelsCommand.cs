namespace XBee.Frames.AtCommands
{
    internal class ScanChannelsCommand : AtCommand
    {
        public const string Name = "SC";

        public ScanChannelsCommand() : base(Name)
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
