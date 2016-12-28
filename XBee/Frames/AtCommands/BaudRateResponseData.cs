using System;
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class BaudRateResponseData : AtCommandResponseFrameData
    {
        public byte[] BaudRateData { get; set; }

        [Ignore]
        public uint BaudRate
        {
            get
            {
                uint baudRate = 0;

                if (BaudRateData.Length == 1)
                    baudRate = BaudRateData[0];
                else if (BaudRateData.Length == 2)
                    baudRate = BitConverter.ToUInt16(BaudRateData, 0);
                else if (BaudRateData.Length == 4)
                    baudRate = BitConverter.ToUInt32(BaudRateData, 0);

                if (baudRate < byte.MaxValue)
                {
                    var cannedBaudRate = (BaudRate) baudRate;

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
                    }
                }

                return baudRate;
            }
        }
    }
}
