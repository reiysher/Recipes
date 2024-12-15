using EventsSync.Api.Shared.Entities;

namespace EventsSync.Api.Shared.Abstractions;

public interface IUserRepository
{
    Task<User?> Get(Guid userId, CancellationToken cancellationToken);

    Task Save(User user, CancellationToken cancellationToken);
}