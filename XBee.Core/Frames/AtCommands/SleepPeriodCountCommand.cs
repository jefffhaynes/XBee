namespace XBee.Frames.AtCommands
{
    internal class SleepPeriodCountCommand : AtCommand
    {
        public const string Name = "SN";

        public SleepPeriodCountCommand() : base(Name)
        {
        }

        public SleepPeriodCountCommand(ushort period) : this()
        {
            Parameter = period;
        }
    }
}
