using EventsAsync.Api.Shared.Abstractions;

namespace EventsAsync.Api.Shared.Entities;

public sealed record UserCreated(
    Guid UserId,
    string FirstName,
    string LastName,
    string? MiddleName,
    string Email)
    : DomainEvent;