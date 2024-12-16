using EventsAsync.Api.Shared.Abstractions;
using EventsAsync.Api.Shared.EventSourcing.Abstractions;

namespace EventsAsync.Api.Shared.Persistence;

internal sealed class UnitOfWork(
    IEventStore eventStore,
    ApplicationDbContext dbContext,
    IEnumerable<IInlineProjection> inlineProjections,
    IEnumerable<ILiveProjection> liveProjections)
    : IUnitOfWork
{
    public async Task<int> Commit(CancellationToken cancellationToken)
    {
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
            foreach (var domainEvent in domainEvents
                         .Where(domainEvent => projection.RelevantEventTypes.Contains(domainEvent.GetType())))
            {
                await projection.Handle(domainEvent, cancellationToken);
            }
        }
    }
}