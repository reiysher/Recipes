using Microsoft.EntityFrameworkCore;
using StateAsync.Api.Shared.Abstractions;

namespace StateAsync.Api.Shared.Persistence;

internal sealed class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    public async Task<int> Commit(CancellationToken cancellationToken)
    {
        var changedAggregatesTypes = dbContext.ChangeTracker
            .Entries<IProjectionSource>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .Select(entityEntry => entityEntry.Entity.GetType().AssemblyQualifiedName!)
            .ToArray();

        var syncEntities = await dbContext
            .Set<SyncEntity>()
            .Where(e => changedAggregatesTypes.Contains(e.Type))
            .ToListAsync(cancellationToken);

        syncEntities.ForEach(syncEntity => syncEntity.HasChanges = true);

        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}
