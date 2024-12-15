namespace EventsSync.Api.Shared.Entities;

public sealed class UserReadModel
{
    public required Guid Id { get; set; }

    public required string Email { get; set; }

    public required string FullName { get; set; }

    public required string ShortName { get; set; }
}
