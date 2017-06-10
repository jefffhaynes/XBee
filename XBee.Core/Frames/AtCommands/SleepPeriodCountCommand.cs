namespace XBee.Frames.AtCommands
{
    internal class SleepPeriodCountCommand : AtCommand
    {
        public SleepPeriodCountCommand() : base("SN")
        {
        }

        public SleepPeriodCountCommand(ushort period) : this()
        {
            Parameter = period;
        }
    }
}
