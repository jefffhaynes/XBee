using System.Collections.Generic;

namespace XBee.Frames
{
    public interface IRxIndicatorSampleFrame : IRxIndicatorFrame
    {
        DigitalSampleState DigitalSampleState { get; set; }

        IEnumerable<AnalogSample> GetAnalogSamples();
    }
}
