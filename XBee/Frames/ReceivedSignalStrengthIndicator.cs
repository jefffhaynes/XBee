using BinarySerialization;

namespace XBee.Frames
{
    public class ReceivedSignalStrengthIndicator
    {
        private const byte MinSignalLoss = 0x17;
        private const byte MaxSignalLoss = 0x64;
        private const byte SignalLossRange = MaxSignalLoss - MinSignalLoss;
        private const byte SignalLossBandSize = SignalLossRange / 3;
        private const byte SignalLossHighThreshold = MaxSignalLoss - SignalLossBandSize;
        private const byte SignalLossLowThreshold = MinSignalLoss + SignalLossBandSize;

        public byte SignalLoss { get; set; }

        [Ignore]
        public SignalStrength SignalStrength
        {
            get
            {
                SignalStrength signalStrength;
                if (SignalLoss > SignalLossHighThreshold)
                    signalStrength = SignalStrength.Low;
                else if (SignalLoss < SignalLossLowThreshold)
                    signalStrength = SignalStrength.High;
                else signalStrength = SignalStrength.Medium;

                return signalStrength;
            }
        }

        public override string ToString()
        {
            return SignalStrength.ToString();
        }
    }
}
