using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class SleepOptionsCommand : AtCommand
    {
        public SleepOptionsCommand() : base("SO")
        {
        }

        public SleepOptionsCommand(SleepOptions options) : this()
        {
            Options = options;
        }

        [Ignore]
        public SleepOptions? Options
        {
            get { return Parameter as SleepOptions?; }
            set { Parameter = value; }
        }
    }
}
