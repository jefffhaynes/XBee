using System;
using BinarySerialization;
using XBee.Frames.AtCommands;

namespace XBee.Frames
{
    public class AtCommandResponseFrame : CommandResponseFrameContent 
    {
        private const int AtCommandFieldLength = 2;

        [FieldLength(AtCommandFieldLength)]
        public string AtCommand { get; set; }

        public AtCommandStatus Status { get; set; }

        [Subtype("AtCommand", "ND", typeof(NetworkDiscoveryResponseData))]
        [Subtype("AtCommand", "HV", typeof(HardwareVersionResponseData))]
        [Subtype("AtCommand", "CE", typeof(CoordinatorEnableResponseData))]
        [Subtype("AtCommand", "NI", typeof(NodeIdentifierResponseData))]
        [Subtype("AtCommand", "SH", typeof(PrimitiveResponseData<UInt32>))]
        [Subtype("AtCommand", "SL", typeof(PrimitiveResponseData<UInt32>))]

        public AtCommandResponseFrameData Data { get; set; }
    }
}
