using System.Collections.Generic;
using XBee.Frames;

namespace XBee
{
    public class SourcedSampleReceivedEventArgs : SampleReceivedEventArgs
    {
        public SourcedSampleReceivedEventArgs(NodeAddress address, DigitalSampleState digitalSampleState, IEnumerable<AnalogSample> analogSamples)
            : base(digitalSampleState, analogSamples)
        {
            Address = address;
        }

        public NodeAddress Address { get; private set; }
    }
}
