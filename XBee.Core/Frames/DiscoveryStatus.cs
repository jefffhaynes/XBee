using JetBrains.Annotations;

namespace XBee.Frames
{
    [PublicAPI]
    public enum DiscoveryStatus : byte
    {
        NoDiscoveryOverhead = 0x00,
        RouteDiscovery = 0x02
    }
}
