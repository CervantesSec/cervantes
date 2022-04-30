using System.Linq;
using System.Threading.Tasks;


namespace Cervantes.Contracts
{
    public interface IGenericManager<T> where T : class
    {
        IApplicationDbContext Context { get; }
        T Add(T entity);
        Task<T> AddAsync(T entity);
        IQueryable<T> GetAll();
        T GetById(object[] key);
        T GetById(int id);
        T Remove(T entity);
    }
}