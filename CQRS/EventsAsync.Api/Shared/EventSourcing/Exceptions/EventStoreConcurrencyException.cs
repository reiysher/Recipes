namespace EventsAsync.Api.Shared.EventSourcing.Exceptions;

public sealed class EventStoreConcurrencyException(Guid streamId)
    : Exception($"Concurrency conflict. Stream [{streamId}] has been modified by another process.");