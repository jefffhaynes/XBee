using System;
using JetBrains.Annotations;

namespace XBee
{
    [PublicAPI]
    public class XBeeException : Exception
    {
        public XBeeException(string message) : base(message)
        {
        }
    }
}
