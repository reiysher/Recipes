using EventsAsync.Api.Shared.Abstractions;

namespace EventsAsync.Api.Shared.Entities;

public sealed record UserPersonalInfoChanged(
    Guid UserId,
    string FirstName,
    string LastName,
    string? MiddleName)
    : DomainEvent;