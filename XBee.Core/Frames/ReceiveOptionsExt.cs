using System;
using JetBrains.Annotations;

namespace XBee.Frames
{
    [PublicAPI]
    [Flags]
    public enum ReceiveOptionsExt : byte
    {
        PacketWasAcknowledged = 0x01,
        BroadcastedPacket = 0x02,
        PacketEncrypted = 0x20,
        RepeaterMode = 0x40,
        PointMultipointMode = 0x80,
        DigiMeshMode = RepeaterMode | PointMultipointMode
    }
}
