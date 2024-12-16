using System.ComponentModel.DataAnnotations.Schema;

namespace EventsAsync.Api.Shared.Abstractions;

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public int Version { get; set; }

    protected void RaiseDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Add(eventItem);
        Version++;
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public virtual void Apply(IDomainEvent domainEvent)
    {
        // empty by default
    }
}
