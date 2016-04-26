using System.Collections.Generic;
using XBee.Frames;

namespace XBee
{
    public class SourcedSampleReceivedEventArgs : SampleReceivedEventArgs
    {
        internal SourcedSampleReceivedEventArgs(NodeAddress address, DigitalSampleState digitalSampleState, IEnumerable<AnalogSample> analogSamples)
            : base(digitalSampleState, analogSamples)
        {
            Address = address;
        }

        public NodeAddress Address { get; }
    }
}
