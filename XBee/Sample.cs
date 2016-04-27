using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using XBee.Frames;

namespace XBee
{
    /// <summary>
    /// Represents a sample returned from an XBee node.
    /// </summary>
    public class Sample
    {
        internal Sample(DigitalSampleChannels digitalChannels, DigitalSampleState digitalSampleState, 
            AnalogSampleChannels analogChannels, IEnumerable<AnalogSample> analogSamples)
        {
            DigitalChannels = digitalChannels;
            DigitalSampleState = digitalSampleState;
            AnalogChannels = analogChannels;
            AnalogSamples = new ReadOnlyCollection<AnalogSample>(analogSamples.ToList());
        }

        /// <summary>
        /// Bit-mask indicating which digital channels are included in the sample.
        /// </summary>
        public DigitalSampleChannels DigitalChannels { get; }

        /// <summary>
        /// Bit-mask indicating which digital channels are high.
        /// </summary>
        public DigitalSampleState DigitalSampleState { get; }

        /// <summary>
        /// Bit-mask indicating which analog channels are included in the sample.
        /// </summary>
        public AnalogSampleChannels AnalogChannels { get; }

        /// <summary>
        /// Collection of analog readings with one reading per active analog channel.
        /// </summary>
        public ReadOnlyCollection<AnalogSample> AnalogSamples { get; }

        public override string ToString()
        {
            var builder = new StringBuilder($"DIO: {DigitalSampleState}");

            if (AnalogSamples.Any())
            {
                var analogSamples = string.Join(", ", AnalogSamples);
                builder.AppendFormat(", ADC: {0}", analogSamples);
            }

            return builder.ToString();
        }
    }
}
