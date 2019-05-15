using System;
using JetBrains.Annotations;

namespace XBee
{
    [PublicAPI]
    public class ModemStatusChangedEventArgs : EventArgs
    {
        internal ModemStatusChangedEventArgs(ModemStatus status)
        {
            Status = status;
        }

        public ModemStatus Status { get; }
    }
}
