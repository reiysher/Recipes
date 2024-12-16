namespace EventsAsync.Api.Shared.EventSourcing.Entities;

internal sealed class EventStream
{
    public required Guid Id { get; init; }

    public required string Type { get; init; }

    public required long Version { get; set; }

    public required ICollection<EventData> Events { get; init; } = [];
}
