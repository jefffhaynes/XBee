using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace XBee.Classic
{
    public class SerialPortWrapper : ISerialDevice
    {
        private readonly SerialPort _serialPort;

        public SerialPortWrapper(SerialPort serialPort)
        {
            _serialPort = serialPort;
        }


        public void Write(byte[] data)
        {
            _serialPort.BaseStream.Write(data, 0, data.Length);
        }

        public async Task<byte[]> ReadAsync(uint count, CancellationToken cancellationToken)
        {
            var data = new byte[count];
            var read = await _serialPort.BaseStream.ReadAsync(data, 0, data.Length, cancellationToken).ConfigureAwait(false);

            if (read != count)
            {
                throw new InvalidDataException("BUG: Read is non-blocking");
            }

            return data;
        }
    }
}
