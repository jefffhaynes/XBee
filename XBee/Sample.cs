using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using XBee.Frames;

namespace XBee
{
    public class Sample
    {
        internal Sample(DigitalSampleState digitalSampleState, IEnumerable<AnalogSample> analogSamples)
        {
            DigitalSampleState = digitalSampleState;
            AnalogSamples = new ReadOnlyCollection<AnalogSample>(analogSamples.ToList());
        }

        public DigitalSampleState DigitalSampleState { get; private set; }

        public ReadOnlyCollection<AnalogSample> AnalogSamples { get; private set; }

        public override string ToString()
        {
            var builder = new StringBuilder(string.Format("DIO: {0}", DigitalSampleState));

            if (AnalogSamples.Any())
            {
                var analogSamples = string.Join(", ", AnalogSamples);
                builder.AppendFormat(", ADC: {0}", analogSamples);
            }

            return builder.ToString();
        }
    }
}
