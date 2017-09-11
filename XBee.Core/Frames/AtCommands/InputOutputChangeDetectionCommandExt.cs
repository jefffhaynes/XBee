using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class InputOutputChangeDetectionCommandExt : AtCommand
    {
        public InputOutputChangeDetectionCommandExt() : base(InputOutputChangeDetectionCommand.Name)
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
            get => Parameter as DigitalSampleChannels?;
            set => Parameter = value;
        }
    }
}
