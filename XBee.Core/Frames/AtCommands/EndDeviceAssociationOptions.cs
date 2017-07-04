using System;

namespace XBee.Frames.AtCommands
{
    [Flags]
    public enum EndDeviceAssociationOptions : byte
    {
        None = 0x00,
        MatchPan = 0x01,
        MatchChannel = 0x02,
        AutoAssociate = 0x04,
        PollCoordinatorOnPinWake = 0x08
    }
}
