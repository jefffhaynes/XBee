using System;

namespace XBee.Frames
{
    [Flags]
    public enum ReceiveOptionsExt : byte
    {
        PacketWasAcknowledged = 0x01,
        BroadcastedPacket = 0x02,
        RepeaterMode = 0x40,
        PointMultipointMode = 0x80,
        DigiMeshMode = RepeaterMode | PointMultipointMode
    }
}
