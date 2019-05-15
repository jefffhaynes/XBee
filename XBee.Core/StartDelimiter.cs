using JetBrains.Annotations;

namespace XBee
{
    [PublicAPI]
    public enum StartDelimiter : byte
    {
        FrameDelimiter = 0x7e,
        Escape = 0x7d,
        XOn = 0x11,
        XOff = 0x13
    }
}
