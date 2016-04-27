using System.Collections.Generic;
using System.Linq;
using BinarySerialization;
using XBee.Converters;
using XBee.Frames;

namespace XBee
{
    public class RxIndicatorSampleFrameContent
    {
        [FieldOrder(0)]
        public ReceivedSignalStrengthIndicator ReceivedSignalStrengthIndicator { get; set; }

        [FieldOrder(1)]
        public ReceiveOptions ReceiveOptions { get; set; }

        [FieldOrder(2)]
        public byte SampleCount { get; set; }

        [FieldOrder(3)]
        public SampleChannels Channels { get; set; }

        [FieldOrder(4)]
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

        [FieldOrder(5)]
        [FieldCount(Path = "Channels",
            ConverterType = typeof(BitCountingConverter), ConverterParameter = SampleChannels.AllAnalog)]
        public List<ushort> AnalogSamples { get; set; }


        public Sample GetSample()
        {
            var digitalChannels = ConvertToDigitalSampleChannels(Channels);
            var analogChannels = ConvertToAnalogSampleChannels(Channels);
            return new Sample(digitalChannels, DigitalSampleState, analogChannels, GetAnalogSamples());
        }

        private IEnumerable<AnalogSample> GetAnalogSamples()
        {
            if (AnalogSamples == null)
                return Enumerable.Empty<AnalogSample>();

            IEnumerable<SampleChannels> analogChannels = (Channels & SampleChannels.AllAnalog).GetFlagValues();
            return AnalogSamples.Zip(analogChannels, (sample, channel) => new AnalogSample(channel, sample));
        }

        private static DigitalSampleChannels ConvertToDigitalSampleChannels(SampleChannels channels)
        {
            var digitalChannels = DigitalSampleChannels.None;

            if (channels.HasFlag(SampleChannels.Digital0))
                digitalChannels |= DigitalSampleChannels.Input0;
            if (channels.HasFlag(SampleChannels.Digital1))
                digitalChannels |= DigitalSampleChannels.Input1;
            if (channels.HasFlag(SampleChannels.Digital2))
                digitalChannels |= DigitalSampleChannels.Input2;
            if (channels.HasFlag(SampleChannels.Digital3))
                digitalChannels |= DigitalSampleChannels.Input3;
            if (channels.HasFlag(SampleChannels.Digital4))
                digitalChannels |= DigitalSampleChannels.Input4;
            if (channels.HasFlag(SampleChannels.Digital5))
                digitalChannels |= DigitalSampleChannels.Input5;
            if (channels.HasFlag(SampleChannels.Digital6))
                digitalChannels |= DigitalSampleChannels.Input6;
            if (channels.HasFlag(SampleChannels.Digital7))
                digitalChannels |= DigitalSampleChannels.Input7;
            if (channels.HasFlag(SampleChannels.Digital8))
                digitalChannels |= DigitalSampleChannels.Input8;

            return digitalChannels;
        }

        private static AnalogSampleChannels ConvertToAnalogSampleChannels(SampleChannels channels)
        {
            var analogChannels = AnalogSampleChannels.None;

            if (channels.HasFlag(SampleChannels.Analog0))
                analogChannels |= AnalogSampleChannels.Input0;
            if (channels.HasFlag(SampleChannels.Analog1))
                analogChannels |= AnalogSampleChannels.Input1;
            if (channels.HasFlag(SampleChannels.Analog2))
                analogChannels |= AnalogSampleChannels.Input2;
            if (channels.HasFlag(SampleChannels.Analog3))
                analogChannels |= AnalogSampleChannels.Input3;
            if (channels.HasFlag(SampleChannels.Analog4))
                analogChannels |= AnalogSampleChannels.Input4;
            if (channels.HasFlag(SampleChannels.Analog5))
                analogChannels |= AnalogSampleChannels.Input5;

            return analogChannels;
        }
    }
}
