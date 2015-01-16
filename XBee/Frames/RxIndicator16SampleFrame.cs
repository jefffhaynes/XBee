using System.Collections.Generic;
using System.Linq;
using BinarySerialization;
using XBee.Converters;

namespace XBee.Frames
{
    public class RxIndicator16SampleFrame : FrameContent, IRxIndicatorSampleFrame
    {
        [FieldOrder(0)]
        public ShortAddress Source { get; set; }

        [FieldOrder(1)]
        public ReceivedSignalStrengthIndicator ReceivedSignalStrengthIndicator { get; set; }

        [FieldOrder(2)]
        public ReceiveOptions ReceiveOptions { get; set; }

        [FieldOrder(3)]
        public byte SampleCount { get; set; }

        [FieldOrder(4)]
        public SampleChannels Channels { get; set; }

        [FieldOrder(5)]
        [SerializeWhen("Channels", SampleChannels.Digital0,
            ConverterType = typeof(BitwiseAndConverter), ConverterParameter = SampleChannels.Digital0)]
        [SerializeWhen("Channels", SampleChannels.Digital1,
            ConverterType = typeof(BitwiseAndConverter), ConverterParameter = SampleChannels.Digital1)]
        [SerializeWhen("Channels", SampleChannels.Digital2,
            ConverterType = typeof(BitwiseAndConverter), ConverterParameter = SampleChannels.Digital2)]
        [SerializeWhen("Channels", SampleChannels.Digital3,
            ConverterType = typeof(BitwiseAndConverter), ConverterParameter = SampleChannels.Digital3)]
        [SerializeWhen("Channels", SampleChannels.Digital4,
            ConverterType = typeof(BitwiseAndConverter), ConverterParameter = SampleChannels.Digital4)]
        [SerializeWhen("Channels", SampleChannels.Digital5,
            ConverterType = typeof(BitwiseAndConverter), ConverterParameter = SampleChannels.Digital5)]
        [SerializeWhen("Channels", SampleChannels.Digital6,
            ConverterType = typeof(BitwiseAndConverter), ConverterParameter = SampleChannels.Digital6)]
        [SerializeWhen("Channels", SampleChannels.Digital7,
            ConverterType = typeof(BitwiseAndConverter), ConverterParameter = SampleChannels.Digital7)]
        [SerializeWhen("Channels", SampleChannels.Digital8,
            ConverterType = typeof(BitwiseAndConverter), ConverterParameter = SampleChannels.Digital8)]
        public DigitalSampleState DigitalSampleState { get; set; }

        [FieldOrder(6)]
        [FieldCount(Path = "Channels",
            ConverterType = typeof (BitCountingConverter), ConverterParameter = SampleChannels.AllAnalog)]
        public List<ushort> AnalogSamples { get; set; }


        public Sample GetSample()
        {
            return new Sample(DigitalSampleState, GetAnalogSamples());
        }

        public NodeAddress GetAddress()
        {
            return new NodeAddress(Source);
        }

        private IEnumerable<AnalogSample> GetAnalogSamples()
        {
            IEnumerable<SampleChannels> analogChannels = (Channels & SampleChannels.AllAnalog).GetFlagValues();
            return AnalogSamples.Zip(analogChannels, (sample, channel) => new AnalogSample(channel, sample));
        }
    }
}