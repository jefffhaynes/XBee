﻿using System;
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class TcpClientConnectionTimeoutResponseData : AtCommandResponseFrameData
    {
        public ushort Value { get; set; }

        [Ignore]
        public TimeSpan Timeout => TimeSpan.FromMilliseconds(Value * TcpClientConnectionTimeoutCommand.ValueMsScale);
    }
}
