namespace EventsSync.Api.Shared.Abstractions;

public interface IAggregate : IAggregateRoot
{
    int Version { get; set; }

    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();

    void Apply(IDomainEvent domainEvent);
}
