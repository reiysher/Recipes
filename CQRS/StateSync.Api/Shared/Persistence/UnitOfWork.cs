using Microsoft.EntityFrameworkCore;
using StateSync.Api.Shared.Abstractions;

namespace StateSync.Api.Shared.Persistence;

internal sealed class UnitOfWork(
    ApplicationDbContext dbContext,
    IEnumerable<IProjectionHandler> projectionHandlers)
    : IUnitOfWork, IDisposable
{
    public async Task<int> Commit(CancellationToken cancellationToken)
    {
        var changedAggregates = dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .Select(e => e.Entity)
            .ToArray();

        foreach (var aggregate in changedAggregates)
        {
            foreach (var projectionHandler in projectionHandlers)
            {
                if (projectionHandler.CanHandle(aggregate))
                {
                    await projectionHandler.Handle(aggregate);
                }
            }
        }

        return await dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        dbContext.Dispose();
    }
}
