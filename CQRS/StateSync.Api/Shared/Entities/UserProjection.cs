using StateSync.Api.Shared.Abstractions;

namespace StateSync.Api.Shared.Entities;

public sealed class UserProjection : IProjection
{
    public required Guid Id { get; set; }

    public required string Email { get; set; }

    public required string FullName { get; set; }

    public required string ShortName { get; set; }
}
