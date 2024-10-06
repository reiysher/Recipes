using StateAsync.Api.Shared.Entities;

namespace StateAsync.Api.Shared.Abstractions;

public interface IUserRepository
{
    Task<User?> Get(Guid userId, CancellationToken cancellationToken);

    void Insert(User user);
}