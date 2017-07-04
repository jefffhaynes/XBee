using System;

namespace XBee
{
    public class ModemStatusChangedEventArgs : EventArgs
    {
        internal ModemStatusChangedEventArgs(ModemStatus status)
        {
            Status = status;
        }

        public ModemStatus Status { get; }
    }
}
