using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace XBee
{
    internal class AtCommandWriter
    {
        private static readonly TimeSpan AckTimeout = TimeSpan.FromSeconds(3);

        private readonly Stream _stream;

        public AtCommandWriter(Stream stream)
        {
            _stream = stream;
        }

        public async Task WriteAsync(string command)
        {
            var writer = new StreamWriter(_stream, Encoding.UTF8, 4096, true);

            await writer.WriteAsync(command).ConfigureAwait(false);
            await writer.FlushAsync().ConfigureAwait(false);

            var start = DateTime.Now;

            while (DateTime.Now - start > AckTimeout)
            {
                if (await IsByteAsync(_stream, (byte) 'O').ConfigureAwait(false))
                {
                    if (await IsByteAsync(_stream, (byte)'K').ConfigureAwait(false))
                    {
                        if (await IsByteAsync(_stream, (byte)'\r').ConfigureAwait(false))
                        {
                            break;
                        }
                    }
                }
            }
        }

        private static async Task<bool> IsByteAsync(Stream stream, byte value)
        {
            var data = new byte[1];
            await stream.ReadAsync(data, 0, data.Length).ConfigureAwait(false);
            return data[0] == value;
        }
    }
}
