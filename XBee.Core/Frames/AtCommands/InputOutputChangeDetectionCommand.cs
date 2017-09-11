using System;
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class InputOutputChangeDetectionCommand : AtCommand
    {
        public const string Name = "IC";

        public InputOutputChangeDetectionCommand() : base(Name)
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
