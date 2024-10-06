using Microsoft.EntityFrameworkCore;
using StateAsync.Api.Shared.Abstractions;
using StateAsync.Api.Shared.Entities;

namespace StateAsync.Api.Shared.Persistence;

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