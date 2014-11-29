namespace XBee.Frames.AtCommands
{
    public class InputOutputChangeDetectionCommand : AtCommandFrame
    {
        public InputOutputChangeDetectionCommand() : base("IC")
        {
        }

        public InputOutputChangeDetectionCommand(DigitalSampleChannels channels) : this()
        {
            Channels = channels;
        }

        public DigitalSampleChannels? Channels
        {
            get { return Parameter as DigitalSampleChannels?; }
            set { Parameter = value; }
        }
    }
}
