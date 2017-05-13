namespace XBee.Frames.AtCommands
{
    public class SleepPeriodCountCommand : AtCommand
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
