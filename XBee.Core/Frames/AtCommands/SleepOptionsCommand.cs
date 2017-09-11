using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class SleepOptionsCommand : AtCommand
    {
        public const string Name = "SO";

        public SleepOptionsCommand() : base(Name)
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
