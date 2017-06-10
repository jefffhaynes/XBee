using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace XBee.Universal
{
    public class SerialDeviceWrapper : ISerialDevice
    {
        private readonly SerialDevice _serialDevice;

        public SerialDeviceWrapper(SerialDevice serialDevice)
        {
            _serialDevice = serialDevice;
        }

        public void Write(byte[] data)
        {
            using (var writer = new DataWriter(_serialDevice.OutputStream))
            {
                writer.WriteBytes(data);
                writer.StoreAsync().AsTask().Wait();
                writer.DetachStream();
            }
        }

        public async Task<byte[]> ReadAsync(uint count, CancellationToken cancellationToken)
        {
            // needed because LoadAsync incorrectly completes sometimes
            var bufferStream = new MemoryStream();

            do
            {
                using (var reader = new DataReader(_serialDevice.InputStream))
                {
                    var read = await reader.LoadAsync(count - (uint)bufferStream.Length);

                    var data = new byte[read];
                    reader.ReadBytes(data);

                    await bufferStream.WriteAsync(data, 0, data.Length, cancellationToken);

                    if (bufferStream.Length < count)
                    {
                        await Task.Delay(100, cancellationToken);
                    }

                    reader.DetachStream();
                }
            } while (bufferStream.Length < count);

            return bufferStream.ToArray();
        }
    }
}
