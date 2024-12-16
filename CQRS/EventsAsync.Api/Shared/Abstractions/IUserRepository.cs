using EventsAsync.Api.Shared.Entities;

namespace EventsAsync.Api.Shared.Abstractions;

public interface IUserRepository
{
    Task<User?> Get(Guid userId, CancellationToken cancellationToken);

    Task Save(User user, CancellationToken cancellationToken);
}