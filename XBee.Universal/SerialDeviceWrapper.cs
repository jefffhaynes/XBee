using System;
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
            using (var reader = new DataReader(_serialDevice.InputStream))
            {
                await reader.LoadAsync(count).AsTask(cancellationToken).ConfigureAwait(false);

                var data = new byte[count];
                reader.ReadBytes(data);
                reader.DetachStream();
                return data;
            }
        }
    }
}
