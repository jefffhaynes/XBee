using System.Threading;
using System.Threading.Tasks;

namespace XBee
{
    public interface ISerialDevice
    {
        void Write(byte[] data);
        Task<byte[]> ReadAsync(uint count, CancellationToken cancellationToken);
    }
}
