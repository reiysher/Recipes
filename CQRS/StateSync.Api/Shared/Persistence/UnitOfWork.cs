using Microsoft.EntityFrameworkCore;
using StateSync.Api.Shared.Abstractions;

namespace StateSync.Api.Shared.Persistence;

internal sealed class UnitOfWork(
    ApplicationDbContext dbContext,
    IEnumerable<ISyncProjectionHandler> projectionHandlers)
    : IUnitOfWork
{
    public async Task<int> Commit(CancellationToken cancellationToken)
    {
        var changedEntities = dbContext.ChangeTracker
            .Entries<IProjectionSource>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .Select(e => e.Entity)
            .ToArray();

        foreach (var entity in changedEntities)
        {
            foreach (var projectionHandler in projectionHandlers)
            {
                if (projectionHandler.CanHandle(entity))
                {
                    await projectionHandler.Handle(entity);
                }
            }
        }

        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}
