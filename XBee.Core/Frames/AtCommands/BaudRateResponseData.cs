using System.IO;
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class BaudRateResponseData : AtCommandResponseFrameData
    {
        public uint BaudRateValue { get; set; }

        [Ignore]
        public uint BaudRate
        {
            get
            {
                if (BaudRateValue > byte.MaxValue)
                    return BaudRateValue;

                var cannedBaudRate = (BaudRate)BaudRateValue;

                switch (cannedBaudRate)
                {
                    case AtCommands.BaudRate.Baudrate1200:
                        return 1200;
                    case AtCommands.BaudRate.Baudrate2400:
                        return 2400;
                    case AtCommands.BaudRate.Baudrate4800:
                        return 4800;
                    case AtCommands.BaudRate.Baudrate9600:
                        return 9600;
                    case AtCommands.BaudRate.Baudrate19200:
                        return 19200;
                    case AtCommands.BaudRate.Baudrate38400:
                        return 38400;
                    case AtCommands.BaudRate.Baudrate57600:
                        return 57600;
                    case AtCommands.BaudRate.Baudrate115200:
                        return 115200;
                    default:
                        throw new InvalidDataException("Unknown canned baud rate.");
                }
            }
        }
    }
}
