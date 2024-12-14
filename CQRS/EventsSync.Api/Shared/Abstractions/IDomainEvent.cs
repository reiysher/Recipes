namespace EventsSync.Api.Shared.Abstractions;

public interface IDomainEvent
{
    Guid Id { get; }

    DateTimeOffset OccurredOn { get; }
}
