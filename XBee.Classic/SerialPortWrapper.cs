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
            // needed because LoadAsync incorrectly completes sometimes
            var bufferStream = new MemoryStream();

            do
            {
                var data = new byte[count - (uint) bufferStream.Length];
                var read = await _serialPort.BaseStream.ReadAsync(data, 0, data.Length, cancellationToken)
                    .ConfigureAwait(false);


                await bufferStream.WriteAsync(data, 0, read, cancellationToken);

                if (bufferStream.Length < count)
                {
                    await Task.Delay(100, cancellationToken);
                }
            } while (bufferStream.Length < count);

            return bufferStream.ToArray();
        }
    }
}
