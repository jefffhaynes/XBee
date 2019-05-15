using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace XBee
{
    [PublicAPI]
    public interface ISerialDevice
    {
        void Write(byte[] data);
        Task<byte[]> ReadAsync(uint count, CancellationToken cancellationToken);
    }
}
