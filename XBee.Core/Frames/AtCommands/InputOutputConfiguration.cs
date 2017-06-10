namespace XBee.Frames.AtCommands
{
    public enum InputOutputConfiguration : byte
    {
        Disabled = 0x0,
        Special = 0x1,
        AnalogIn = 0x2,
        DigitalIn = 0x3,
        DigitalLow = 0x4,
        DigitalHigh = 0x5
    }
}
