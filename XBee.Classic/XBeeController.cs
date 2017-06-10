using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using XBee.Classic;

// ReSharper disable once CheckNamespace
namespace XBee
{
    public class XBeeController : Core.XBeeController
    {
        private readonly SerialPort _serialPort;

        public XBeeController(SerialPort serialPort) : base(new SerialPortWrapper(serialPort))
        {
            _serialPort = serialPort;
            _serialPort.Open();
        }

        public XBeeController(string portName, int baudRate) : this(new SerialPort(portName, baudRate))
        {
        }

        public static Task<XBeeController> FindAndOpenAsync(int baudRate)
        {
            return FindAndOpenAsync(SerialPort.GetPortNames(), baudRate);
        }

        public static async Task<XBeeController> FindAndOpenAsync(IEnumerable<string> portNames, int baudRate)
        {
            foreach (var portName in portNames)
            {
                try
                {
                    var controller = new XBeeController(portName, baudRate);
                    await controller.GetHardwareVersionAsync();
                    return controller;
                }
                catch (InvalidOperationException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (ArgumentOutOfRangeException)
                {
                }
                catch (ArgumentException)
                {
                }
                catch (TimeoutException)
                {
                }
                catch (IOException)
                {
                }
            }

            return null;
        }

        public override void Dispose()
        {
            _serialPort.Dispose();
            base.Dispose();
        }
    }
}
