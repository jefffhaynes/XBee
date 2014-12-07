namespace XBee
{
    public class SourcedSample
    {
        internal SourcedSample(NodeAddress address, Sample sample)
        {
            Address = address;
            Sample = sample;
        }

        public NodeAddress Address { get; private set; }

        public Sample Sample { get; private set; }
    }
}
