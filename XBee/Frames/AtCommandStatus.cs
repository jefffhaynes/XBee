namespace XBee.Frames
{
    public enum AtCommandStatus : byte
    {
        Success = 0x0,
        Error = 0x1,
        InvalidCommand = 0x2,
        InvalidParameter = 0x3,
        NoResponse = 0x4
    }
}
