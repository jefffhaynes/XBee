using System;
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class NetworkDiscoveryTimeoutResponseData : AtCommandResponseFrameData
    {
        public ushort Value { get; set; }

        [Ignore]
        public TimeSpan Timeout => TimeSpan.FromMilliseconds(Value * NetworkDiscoveryTimeoutCommand.ValueMsScale);
    }
}
