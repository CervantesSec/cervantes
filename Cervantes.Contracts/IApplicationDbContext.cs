using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;


namespace Cervantes.Contracts;

public interface IApplicationDbContext
{
    EntityEntry Entry(object entity);

    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

    int SaveChanges();
    Task<int> SaveChangesAsync();
    Task<int> SaveChangesNoAuditAsync();


    //DbSet Set(Type entityType);

    DbSet<TEntity> Set<TEntity>() where TEntity : class;
}