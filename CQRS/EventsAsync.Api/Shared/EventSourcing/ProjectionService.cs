using EventsAsync.Api.Shared.EventSourcing.Abstractions;
using EventsAsync.Api.Shared.EventSourcing.Entities;
using EventsAsync.Api.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventsAsync.Api.Shared.EventSourcing;

internal sealed class ProjectionService<TProjection>(IServiceProvider serviceProvider) : BackgroundService
    where TProjection : class, IAsyncProjection
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var eventStore = scope.ServiceProvider.GetRequiredService<IEventStore>();
            var projection = scope.ServiceProvider.GetRequiredService<TProjection>();

            var checkpoint = await GetCheckpointAsync(dbContext, typeof(TProjection).Name, stoppingToken);

            var events = await eventStore.GetOrderedEvents(
                checkpoint.Value,
                projection.BatchSize,
                projection.RelevantEventTypes,
                stoppingToken);

            if (!events.Any())
            {
                await Task.Delay(projection.Delay, stoppingToken);
            }
            else
            {
                checkpoint.Value = events.Last().OccurredOn;

                foreach (var domainEvent in events)
                {
                    await projection.Handle(domainEvent, stoppingToken);
                }
            }

            await dbContext.SaveChangesAsync(stoppingToken);
        }
    }

    private static async Task<Checkpoint> GetCheckpointAsync(
        ApplicationDbContext dbContext,
        string projectionName,
        CancellationToken cancellationToken)
    {
        var checkpoint = await dbContext
            .Set<Checkpoint>()
            .FirstOrDefaultAsync(c => c.ProjectionName == projectionName, cancellationToken);

        if (checkpoint == null)
        {
            checkpoint = new Checkpoint
            {
                ProjectionName = projectionName,
                Value = DateTimeOffset.MinValue,
            };

            dbContext.Add(checkpoint);
        }

        return checkpoint;
    }
}