using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace XBee.Universal
{
    public class SerialDeviceWrapper : ISerialDevice
    {
        private readonly DataWriter _writer;
        private readonly DataReader _reader;

        public SerialDeviceWrapper(SerialDevice serialDevice)
        {
            _writer = new DataWriter(serialDevice.OutputStream);
            _reader = new DataReader(serialDevice.InputStream);
        }

        public void Write(byte[] data)
        {
            _writer.WriteBytes(data);
            _writer.FlushAsync().AsTask().Wait();
        }

        public async Task<byte[]> Read(uint count, CancellationToken cancellationToken)
        {
            var data = new byte[count];
            await _reader.LoadAsync(count).AsTask(cancellationToken).ConfigureAwait(false);
            _reader.ReadBytes(data);
            return data;
        }
    }
}
