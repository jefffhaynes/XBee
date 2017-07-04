using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using XBee.Classic;
using XBee.Core;

// ReSharper disable once CheckNamespace
namespace XBee
{
    /// <summary>
    ///     Represents the connected XBee controller module.
    /// </summary>
    public class XBeeController : XBeeControllerBase
    {
        private readonly SerialPort _serialPort;

        private XBeeController(SerialPort serialPort) : base(new SerialPortWrapper(serialPort))
        {
            _serialPort = serialPort;
        }

        public XBeeController() : this(new SerialPort())
        {
        }

        public static Task<XBeeController> FindAndOpenAsync(int baudRate)
        {
            return FindAndOpenAsync(SerialPort.GetPortNames(), baudRate);
        }

        public Task OpenAsync(string portName, int baudRate)
        {
            _serialPort.PortName = portName;
            _serialPort.BaudRate = baudRate;
            _serialPort.Open();

            return GetHardwareVersionAsync();
        }

        public static async Task<XBeeController> FindAndOpenAsync(IEnumerable<string> portNames, int baudRate)
        {
            foreach (var portName in portNames)
            {
                try
                {
                    var controller = new XBeeController();
                    await controller.OpenAsync(portName, baudRate);
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