using JetBrains.Annotations;

namespace XBee.Frames
{
    [PublicAPI]
    public enum RemoteAtCommandOptions : byte
    {
        None = 0x0,
        Commit = 0x2
    }
}
