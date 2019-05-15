using JetBrains.Annotations;

namespace XBee
{
    [PublicAPI]
    public enum XBeeProtocol
    {
        ZigBee,
        Raw,
        WiFi,
        DigiMesh,
        XCite,
        XTend,
        XTendDigiMesh,
        SmartEnergy,
        DigiPoint,
        ZNet,
        XC,
        XLR,
        XLRDigiMesh,
        SX,
        XLRModule,
        Cellular,
        CellularIoT,
        Unknown
    }
}
