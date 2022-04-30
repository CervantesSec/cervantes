using System.Threading.Tasks;

namespace Cervantes.Contracts
{
    public interface IPDFManager
    {
        Task<byte[]> Create();
    }
}
