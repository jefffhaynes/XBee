using System;

namespace XBee
{
    public class SmsReceivedEventArgs : EventArgs
    {
        public SmsReceivedEventArgs(string phoneNumber, string message)
        {
            PhoneNumber = phoneNumber;
            Message = message;
        }

        public string PhoneNumber { get; }

        public string Message { get; }
    }
}
