using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class SleepModeCommand : AtCommand
    {
        public SleepModeCommand() : base("SM")
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
