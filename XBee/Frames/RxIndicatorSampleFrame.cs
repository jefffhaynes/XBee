using System.Collections.Generic;
using BinarySerialization;
using XBee.Converters;

namespace XBee.Frames
{
    public class RxIndicatorSampleFrame : FrameContent
    {
        public LongAddress Source { get; set; }

        public ReceivedSignalStrengthIndicator ReceivedSignalStrengthIndicator { get; set; }

        public ReceiveOptions ReceiveOptions { get; set; }

        public byte SampleCount { get; set; }

        public SampleChannels Channels { get; set; }

        [SerializeWhen("Channels", SampleChannels.Digital0,
            ConverterType = typeof (BitwiseAndConverter), ConverterParameter = SampleChannels.Digital0)]
        [SerializeWhen("Channels", SampleChannels.Digital1,
            ConverterType = typeof (BitwiseAndConverter), ConverterParameter = SampleChannels.Digital1)]
        [SerializeWhen("Channels", SampleChannels.Digital2,
            ConverterType = typeof (BitwiseAndConverter), ConverterParameter = SampleChannels.Digital2)]
        [SerializeWhen("Channels", SampleChannels.Digital3,
            ConverterType = typeof (BitwiseAndConverter), ConverterParameter = SampleChannels.Digital3)]
        [SerializeWhen("Channels", SampleChannels.Digital4,
            ConverterType = typeof (BitwiseAndConverter), ConverterParameter = SampleChannels.Digital4)]
        [SerializeWhen("Channels", SampleChannels.Digital5,
            ConverterType = typeof (BitwiseAndConverter), ConverterParameter = SampleChannels.Digital5)]
        [SerializeWhen("Channels", SampleChannels.Digital6,
            ConverterType = typeof (BitwiseAndConverter), ConverterParameter = SampleChannels.Digital6)]
        [SerializeWhen("Channels", SampleChannels.Digital7,
            ConverterType = typeof (BitwiseAndConverter), ConverterParameter = SampleChannels.Digital7)]
        [SerializeWhen("Channels", SampleChannels.Digital8,
            ConverterType = typeof (BitwiseAndConverter), ConverterParameter = SampleChannels.Digital8)]
        public DigitalSampleState DigitalSampleState { get; set; }

        [FieldCount(Path = "Channels",
            ConverterType = typeof(BitCountingConverter), ConverterParameter = SampleChannels.AllAnalog)]
        public List<ushort> AnalogSamples { get; set; } 
    }
}
