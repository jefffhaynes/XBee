using System;

namespace XBee.Frames.AtCommands
{
    [Flags]
    public enum CoordinatorAssociationOptions : byte
    {
        None = 0x00,
        PerformActiveScan = 0x01,
        PerformEnergyScan = 0x02,
        AllowAssociation = 0x04
    }
}
