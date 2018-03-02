using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace XBee
{
    internal class AtCommandWriter
    {
        private const string AckResponse = "OK\r";

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

            var response = new byte[AckResponse.Length];
            await _stream.ReadAsync(response, 0, response.Length).ConfigureAwait(false);

            if (Encoding.UTF8.GetString(response) != AckResponse)
            {
                throw new InvalidDataException();
            }
        }
    }
}
