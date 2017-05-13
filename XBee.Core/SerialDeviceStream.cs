using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XBee
{
    public class SerialDeviceStream : Stream
    {
        private readonly ISerialDevice _serialDevice;

        public SerialDeviceStream(ISerialDevice serialDevice)
        {
            _serialDevice = serialDevice;
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), offset, "Must be zero.");
            }

            if (count != buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "Must be equal to buffer length.");
            }

            var data = await _serialDevice.Read((uint)count, cancellationToken).ConfigureAwait(false);
            Array.Copy(data, buffer, data.Length);
            return data.Length;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), offset, "Must be zero.");
            }

            if (count != buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "Must be equal to buffer length.");
            }

            _serialDevice.Write(buffer);
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }
    }
}
