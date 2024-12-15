using EventsSync.Api.Shared.Abstractions;
using EventsSync.Api.Shared.EventSourcing;
using EventsSync.Api.Shared.EventSourcing.Abstractions;

namespace EventsSync.Api.Shared.Persistence;

internal sealed class UnitOfWork(
    IEventStore eventStore,
    ApplicationDbContext dbContext,
    IEnumerable<IInlineProjection> inlineProjections,
    IEnumerable<ILiveProjection> liveProjections)
    : IUnitOfWork
{
    public async Task<int> Commit(CancellationToken cancellationToken)
    {
        // todo: uof needs to aggregate changes (events too).
        // todo: find a way to add changes while saving aggregate
        // todo: get events from stateful aggregates
        // todo: combine stateful and event sourcing aggregates
        IReadOnlyCollection<IDomainEvent> domainEvents = eventStore.GetNewDomainEvents();

        await ProcessDomainEvents(domainEvents, inlineProjections, cancellationToken);

        var result = await dbContext.SaveChangesAsync(cancellationToken);

        await ProcessDomainEvents(domainEvents, liveProjections, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return result;
    }

    private static async Task ProcessDomainEvents(
        IReadOnlyCollection<IDomainEvent> domainEvents,
        IEnumerable<IBaseProjection> projections,
        CancellationToken cancellationToken)
    {
        foreach (var projection in projections)
        {
            foreach (var domainEvent in domainEvents.Where(domainEvent => projection.CanHandle(domainEvent.GetType())))
            {
                await projection.Handle(domainEvent, cancellationToken);
            }
        }
    }
}