using System.Threading.Tasks;
using XBee.Frames.AtCommands;

namespace XBee
{
    public interface IAssociationIndicator
    {
        Task<AssociationIndicator> GetAssociationAsync();
    }
}
