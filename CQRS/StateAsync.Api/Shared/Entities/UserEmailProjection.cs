using StateAsync.Api.Shared.Abstractions;

namespace StateAsync.Api.Shared.Entities;

public sealed class UserEmailProjection : IProjection
{
    public required Guid Id { get; set; }

    public required string Email { get; set; }
}
