using Microsoft.EntityFrameworkCore;
using StateAsync.Api.Shared.Abstractions;
using StateAsync.Api.Shared.Entities;

namespace StateAsync.Api.Shared.Persistence;

internal sealed class UserAsyncProjectionHandler(ApplicationDbContext dbContext) : IAsyncProjectionHandler
{
    public bool CanHandle(IProjectionSource aggregate) => aggregate is User;

    public async Task Handle(IProjectionSource aggregate)
    {
        if (aggregate is not User user)
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
