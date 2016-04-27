namespace XBee.Frames.AtCommands
{
    public enum HardwareVersion : byte
    {
        XBeeSeries1 = 0x17,    // XB24_AXX_XX
        XBeeProSeries1 = 0x18, // XBP24_AXX_XX
        ZNetZigBeeS2 = 0x19,   // XB24_BXIX_XXX
        XBeeProS2 = 0x1A,      // XBP24_BXIX_XXX
        XBeePro900 = 0x1B,     // XBP09_DXIX_XXX
        XBeeProS2B = 0x1E,     // XBP24B
        XBeeProS2C = 0x21,     // XBP24C
        XBee24S2C = 0x22,      // XB24C
        XBeePro900HP = 0x23,   // XSC_GEN3
        XBee24C = 0x2E         // XB24C_TH_DIP
    }
}
