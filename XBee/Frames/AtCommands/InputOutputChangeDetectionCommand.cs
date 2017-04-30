using System;
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class InputOutputChangeDetectionCommand : AtCommand
    {
        public InputOutputChangeDetectionCommand() : base("IC")
        {
        }

        public InputOutputChangeDetectionCommand(DigitalSampleChannels channels) : this()
        {
            Channels = channels;
        }

        [Ignore]
        public DigitalSampleChannels? Channels
        {
            get => Parameter as DigitalSampleChannels?;
            set => Parameter = Convert.ToByte(value);
        }
    }
}
