using Microsoft.EntityFrameworkCore;
using StateSync.Api.Shared.Abstractions;
using StateSync.Api.Shared.Entities;

namespace StateSync.Api.Shared.Persistence;

internal sealed class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    public void Insert(User user)
    {
        dbContext.Add(user);
    }

    public Task<User?> Get(Guid userId, CancellationToken cancellationToken)
    {
        return dbContext.Set<User>().SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }
}