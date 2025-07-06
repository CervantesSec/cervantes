using System;
using System.Linq;
using System.Threading.Tasks;


namespace Cervantes.Contracts;

public interface IGenericManager<T> where T : class
{
    IApplicationDbContext Context { get; }
    T Add(T entity);
    Task<T> AddAsync(T entity);
    T Update(T entity);
    IQueryable<T> GetAll();
    T GetById(object[] key);
    T GetById(Guid id);
    T Remove(T entity);
}