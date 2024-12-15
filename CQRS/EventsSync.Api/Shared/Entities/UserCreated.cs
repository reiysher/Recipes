using EventsSync.Api.Shared.Abstractions;

namespace EventsSync.Api.Shared.Entities;

public sealed record UserCreated(
    Guid Id,
    DateTimeOffset OccurredOn,
    Guid UserId,
    string FirstName,
    string LastName,
    string? MiddleName,
    string Email)
    : IDomainEvent;