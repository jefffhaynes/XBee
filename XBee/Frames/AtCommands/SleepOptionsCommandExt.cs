using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class SleepOptionsCommandExt : AtCommand
    {
        public SleepOptionsCommandExt() : base("SO")
        {
        }

        public SleepOptionsCommandExt(SleepOptionsExt options)
            : this()
        {
            Options = options;
        }

        [Ignore]
        public SleepOptionsExt? Options
        {
            get { return Parameter as SleepOptionsExt?; }
            set { Parameter = value; }
        }
    }
}
