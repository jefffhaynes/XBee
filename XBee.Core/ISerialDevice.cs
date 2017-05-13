using System.Threading;
using System.Threading.Tasks;

namespace XBee
{
    public interface ISerialDevice
    {
        void Write(byte[] data);
        Task<byte[]> Read(uint count, CancellationToken cancellationToken);
    }
}
