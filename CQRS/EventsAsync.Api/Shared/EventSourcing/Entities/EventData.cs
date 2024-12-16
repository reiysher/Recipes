namespace EventsAsync.Api.Shared.EventSourcing.Entities;

internal sealed class EventData
{
    public required Guid Id { get; init; }

    public required string Payload { get; init; }

    public required Guid StreamId { get; init; }

    public required string Type { get; init; }

    public required long Version { get; init; }

    public required DateTimeOffset OccuredOn { get; init; }
}
