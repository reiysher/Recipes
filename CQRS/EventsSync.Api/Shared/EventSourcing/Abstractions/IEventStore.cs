using EventsSync.Api.Shared.Abstractions;

namespace EventsSync.Api.Shared.EventSourcing.Abstractions;

public interface IEventStore
{
    IReadOnlyCollection<IDomainEvent> GetNewDomainEvents();

    Task<TAggregate?> AggregateStream<TAggregate>(Guid streamId, CancellationToken cancellationToken)
        where TAggregate : class, IAggregateRoot;

    Task<bool> Save<TStream>(Guid streamId, TStream aggregate, CancellationToken cancellationToken)
        where TStream : IAggregateRoot;
}
