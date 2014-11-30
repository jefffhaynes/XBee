using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class InputOutputChangeDetectionCommandExt : AtCommand
    {
        public InputOutputChangeDetectionCommandExt() : base("IC")
        {
        }

        public InputOutputChangeDetectionCommandExt(DigitalSampleChannels channels)
            : this()
        {
            Channels = channels;
        }

        [Ignore]
        public DigitalSampleChannels? Channels
        {
            get { return Parameter as DigitalSampleChannels?; }
            set { Parameter = value; }
        }
    }
}
