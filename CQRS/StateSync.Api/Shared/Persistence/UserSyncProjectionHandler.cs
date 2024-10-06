using Microsoft.EntityFrameworkCore;
using StateSync.Api.Shared.Abstractions;
using StateSync.Api.Shared.Entities;

namespace StateSync.Api.Shared.Persistence;

internal sealed class UserSyncProjectionHandler(ApplicationDbContext dbContext) : ISyncProjectionHandler
{
    public bool CanHandle(IProjectionSource entity) => entity is User;

    public async Task Handle(IProjectionSource entity)
    {
        if (entity is not User user)
        {
            return;
        }

        var projection = await dbContext
            .Set<UserProjection>()
            .SingleOrDefaultAsync(p => p.Id == user.Id);

        if (projection == null)
        {
            projection = new UserProjection
            {
                Id = user.Id,
                Email = user.Email,
                ShortName = user.GetShortName(),
                FullName = user.GetFullName(),
            };

            dbContext.Add(projection);
        }
        else
        {
            projection.Email = user.Email;
            projection.ShortName = user.GetShortName();
            projection.FullName = user.GetFullName();
        }
    }
}
