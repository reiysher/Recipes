using EventsAsync.Api.Shared.Abstractions;

namespace EventsAsync.Api.Shared.EventSourcing.Abstractions;

public interface IEventStore
{
    IReadOnlyCollection<IDomainEvent> GetNewDomainEvents();

    Task<IReadOnlyCollection<IDomainEvent>> GetOrderedEvents(
        DateTimeOffset checkpoint,
        int batchSize,
        IReadOnlyCollection<Type> eventTypes,
        CancellationToken cancellationToken);

    Task<TAggregate?> AggregateStream<TAggregate>(Guid streamId, CancellationToken cancellationToken)
        where TAggregate : class, IAggregateRoot;

    Task<bool> Save<TStream>(Guid streamId, TStream aggregate, CancellationToken cancellationToken)
        where TStream : IAggregateRoot;
}
