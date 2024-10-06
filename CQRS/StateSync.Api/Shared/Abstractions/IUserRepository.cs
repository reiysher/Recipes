using StateSync.Api.Shared.Entities;

namespace StateSync.Api.Shared.Abstractions;

public interface IUserRepository
{
    Task<User?> Get(Guid userId, CancellationToken cancellationToken);

    void Insert(User user);
}