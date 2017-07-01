using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace XBee.Universal
{
    /// <summary>
    /// Represents an abstraction layer for serial devices.
    /// </summary>
    public class SerialDeviceWrapper : ISerialDevice
    {
        private readonly SerialDevice _serialDevice;

        /// <summary>
        /// Instantiates a new SerialDeviceWrapper.
        /// </summary>
        /// <param name="serialDevice"></param>
        public SerialDeviceWrapper(SerialDevice serialDevice)
        {
            _serialDevice = serialDevice;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void Write(byte[] data)
        {
            using (var writer = new DataWriter(_serialDevice.OutputStream))
            {
                writer.WriteBytes(data);
                writer.StoreAsync().AsTask().Wait();
                writer.DetachStream();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
