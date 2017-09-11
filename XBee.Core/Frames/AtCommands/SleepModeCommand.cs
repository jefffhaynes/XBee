using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class SleepModeCommand : AtCommand
    {
        public const string Name = "SM";

        public SleepModeCommand() : base(Name)
        {
        }

        public SleepModeCommand(SleepMode mode) : this()
        {
            Mode = mode;
        }

        [Ignore]
        public SleepMode? Mode
        {
            get => Parameter as SleepMode?;
            set => Parameter = value;
        }
    }
}
