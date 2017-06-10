namespace XBee.Frames.AtCommands
{
    public enum ApiMode : byte
    {
        Disabled = 0x0,
        Enabled = 0x1,
        EnabledEscaped = 0x2,
        Bypass = 0x5
    }
}
