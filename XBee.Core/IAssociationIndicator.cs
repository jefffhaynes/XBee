using System.Threading.Tasks;
using XBee.Frames.AtCommands;

namespace XBee
{
    internal interface IAssociationIndicator
    {
        Task<AssociationIndicator> GetAssociationAsync();
    }
}
