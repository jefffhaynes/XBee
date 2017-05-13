namespace XBee
{
    public class SourcedSample : Sourced
    {
        internal SourcedSample(NodeAddress address, Sample sample) : base(address)
        {
            Sample = sample;
        }

        public Sample Sample { get; }
    }
}
