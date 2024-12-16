namespace EventsAsync.Api.Shared.EventSourcing.Entities;

public sealed class Checkpoint
{
    public required string ProjectionName { get; init; }

    public required DateTimeOffset Value { get; set; }
}