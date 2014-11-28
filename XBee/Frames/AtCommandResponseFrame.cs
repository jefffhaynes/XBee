using System;
using BinarySerialization;
using XBee.Frames.AtCommands;

namespace XBee.Frames
{
    public class AtCommandResponseFrame<TResponseData> : CommandResponseFrameContent 
        where TResponseData : AtCommandResponseFrameData
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
        [Subtype("AtCommand", "D0", typeof(InputOutputResponseData))]
        [Subtype("AtCommand", "D1", typeof(InputOutputResponseData))]
        [Subtype("AtCommand", "D2", typeof(InputOutputResponseData))]
        [Subtype("AtCommand", "D3", typeof(InputOutputResponseData))]
        [Subtype("AtCommand", "D4", typeof(InputOutputResponseData))]
        [Subtype("AtCommand", "D5", typeof(InputOutputResponseData))]
        [Subtype("AtCommand", "D6", typeof(InputOutputResponseData))]
        [Subtype("AtCommand", "D7", typeof(InputOutputResponseData))]
        [Subtype("AtCommand", "D8", typeof(InputOutputResponseData))]
        [Subtype("AtCommand", "D9", typeof(InputOutputResponseData))]

        public TResponseData Data { get; set; }
    }

    public class AtCommandResponseFrame : AtCommandResponseFrame<AtCommandResponseFrameData>
    {
    }
}
