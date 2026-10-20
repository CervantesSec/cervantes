using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;


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

    // For work that must be all or nothing across several SaveChanges calls (a data restore).
    DatabaseFacade Database { get; }

    ChangeTracker ChangeTracker { get; }
}