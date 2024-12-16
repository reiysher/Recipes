using EventsAsync.Api.Shared.Abstractions;

namespace EventsAsync.Api.Shared.Entities;

public sealed record UserEmailChanged(
    Guid UserId,
    string Email)
    : DomainEvent;