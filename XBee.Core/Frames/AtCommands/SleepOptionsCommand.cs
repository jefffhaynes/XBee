using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class SleepOptionsCommand : AtCommand
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
            get => Parameter as SleepOptions?;
            set => Parameter = value;
        }
    }
}
