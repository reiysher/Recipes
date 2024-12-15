using EventsSync.Api.Shared.Abstractions;

namespace EventsSync.Api.Shared.Entities;

public sealed record UserPersonalInfoChanged(
    Guid Id,
    DateTimeOffset OccurredOn,
    Guid UserId,
    string FirstName,
    string LastName,
    string? MiddleName)
    : IDomainEvent;