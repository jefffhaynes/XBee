using System.Threading.Tasks;
using JetBrains.Annotations;
using XBee.Frames.AtCommands;

namespace XBee
{
    [PublicAPI]
    internal interface IAssociationIndicator
    {
        Task<AssociationIndicator> GetAssociationAsync();
    }
}
