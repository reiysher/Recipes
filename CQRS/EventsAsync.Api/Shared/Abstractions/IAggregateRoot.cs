namespace EventsAsync.Api.Shared.Abstractions;

public interface IAggregateRoot
{
    int Version { get; set; }

    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();

    void Apply(IDomainEvent domainEvent);
}
