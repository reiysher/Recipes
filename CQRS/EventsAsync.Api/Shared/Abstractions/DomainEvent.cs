namespace EventsAsync.Api.Shared.Abstractions;

public abstract record DomainEvent(Guid Id, DateTimeOffset OccurredOn) : IDomainEvent
{
    protected DomainEvent() : this(Guid.NewGuid(), DateTimeOffset.UtcNow)
    {
    }
}