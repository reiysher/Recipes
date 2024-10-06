using Microsoft.EntityFrameworkCore;
using Quartz;
using StateAsync.Api.Shared.Entities;

namespace StateAsync.Api.Shared.Persistence;

[DisallowConcurrentExecution]
internal sealed class SyncJob(
    TimeProvider timeProvider,
    ApplicationDbContext dbContext,
    IEnumerable<IAsyncProjectionHandler> projectionHandlers) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var utcNow = timeProvider.GetUtcNow().UtcDateTime;

        var syncEntities = await dbContext
            .Set<SyncEntity>()
            .Where(e => e.HasChanges)
            .ToArrayAsync();

        foreach (var syncEntity in syncEntities)
        {
            var type = Type.GetType(syncEntity.Type);
            if (type == typeof(User))
            {
                var changedEntities = await dbContext
                    .Set<User>()
                    .Where(e => e.Updated > syncEntity.LastSyncUtc)
                    .AsNoTracking()
                    .ToArrayAsync();

                foreach (var entity in changedEntities)
                {
                    foreach (var projectionHandler in projectionHandlers)
                    {
                        try
                        {
                            if (!projectionHandler.CanHandle(entity))
                            {
                                continue;
                            }

                            await projectionHandler.Handle(entity);
                            await dbContext.SaveChangesAsync();
                        }
                        catch
                        {
                            dbContext.ChangeTracker.Clear();
                        }
                    }
                }

            }
            else
            {
                continue;
            }

            dbContext.Attach(syncEntity);

            syncEntity.LastSyncUtc = utcNow;
            syncEntity.HasChanges = false;

            await dbContext.SaveChangesAsync();
        }
    }
}