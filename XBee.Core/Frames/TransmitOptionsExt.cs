using System;

namespace XBee.Frames
{
    [Flags]
    public enum TransmitOptionsExt : byte
    {
        DisableAck = 0x1,
        DisableRouteDiscovery = 0x2,
        EnableUnicastNackMessages = 0x4,
        EnableUnicastTracerouteMessages = 0x8,
        // 0x10,
        // 0x20,
        RepeaterMode = 0x40,
        PointMultipointMode = 0x80,
        DigiMeshMode = RepeaterMode | PointMultipointMode
    }
}
