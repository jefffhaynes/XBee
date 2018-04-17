using System;

namespace XBee.Frames.AtCommands
{
    [Flags]
    public enum NetworkResetOptions
    {
        None = 0,
        ResetAllNodes = 1,
    }
}