using Microsoft.EntityFrameworkCore;
using StateSync.Api.Shared.Abstractions;
using StateSync.Api.Shared.Entities;

namespace StateSync.Api.Shared.Persistence;

internal sealed class UserSummaryProjectionHandler(ApplicationDbContext dbContext) : IProjectionHandler
{
    public bool CanHandle(IAggregateRoot aggregate) => aggregate is User;

    public async Task Handle(IAggregateRoot aggregate)
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
