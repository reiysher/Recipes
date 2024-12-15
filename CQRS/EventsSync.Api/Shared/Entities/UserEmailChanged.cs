using EventsSync.Api.Shared.Abstractions;

namespace EventsSync.Api.Shared.Entities;

public sealed record UserEmailChanged(
    Guid Id,
    DateTimeOffset OccurredOn,
    Guid UserId,
    string Email)
    : IDomainEvent;