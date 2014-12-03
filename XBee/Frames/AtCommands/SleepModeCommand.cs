using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class SleepModeCommand : AtCommand
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
            get { return Parameter as SleepMode?; }
            set { Parameter = value; }
        }
    }
}
