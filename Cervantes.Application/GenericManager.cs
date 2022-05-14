using Cervantes.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Cervantes.Application;

/// <summary>
/// Clase genérica de Manager
/// </summary>
public class GenericManager<T> : IGenericManager<T>
    where T : class
{
    /// <summary>
    /// Manager data context
    /// </summary>
    public IApplicationDbContext Context { get; private set; }

    /// <summary>
    /// Manager Contructor
    /// </summary>
    /// <param name="context">data context</param>
    public GenericManager(IApplicationDbContext context)
    {
        Context = context;
    }

    /// <summary>
    /// Add entity to data context
    /// </summary>
    /// <param name="entity">Entity to add</param>
    /// <returns>Added entity</returns>
    public T Add(T entity)
    {
        Context.Set<T>().Add(entity);
        return entity;
    }

    /// <summary>
    /// Add entity to data context async
    /// </summary>
    /// <param name="entity">Entity to add</param>
    /// <returns>Added Entity</returns>
    public async Task<T> AddAsync(T entity)
    {
        await Context.Set<T>().AddAsync(entity);
        return entity;
    }

    /// <summary>
    /// Remove entity from data context
    /// </summary>
    /// <param name="entity">Entity to remove</param>
    /// <returns>Entidad eliminada</returns>
    public T Remove(T entity)
    {
        Context.Set<T>().Remove(entity);
        return entity;
    }

    /// <summary>
    /// Get entity by id key
    /// </summary>
    /// <param name="key">id</param>
    /// <returns>Entity</returns>
    public T GetById(object[] key)
    {
        return Context.Set<T>().Find(key);
    }

    /// <summary>
    /// Get entity by his id
    /// </summary>
    /// <param name="id">id</param>
    /// <returns>Entity</returns>
    public T GetById(int id)
    {
        return GetById(new object[] {id});
    }

    /// <summary>
    /// Get all entities
    /// </summary>
    /// <returns>List of all entities</returns>
    public IQueryable<T> GetAll()
    {
        return Context.Set<T>();
    }
}