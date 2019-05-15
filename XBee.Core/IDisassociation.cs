using System.Threading.Tasks;
using JetBrains.Annotations;

namespace XBee
{
    [PublicAPI]
    internal interface IDisassociation
    {
        Task DisassociateAsync();
    }
}
