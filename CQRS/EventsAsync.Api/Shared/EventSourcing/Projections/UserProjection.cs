using EventsAsync.Api.Shared.Entities;
using EventsAsync.Api.Shared.EventSourcing.Abstractions;
using EventsAsync.Api.Shared.Persistence;

namespace EventsAsync.Api.Shared.EventSourcing.Projections;

internal sealed class UserProjection : BaseProjection<UserReadModel>
{
    private readonly ApplicationDbContext _dbContext;

    public UserProjection(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;

        Projects<UserCreated>(When);
        Projects<UserPersonalInfoChanged>(When);
        Projects<UserEmailChanged>(When);
    }
    private async Task When(UserCreated domainEvent, CancellationToken cancellationToken)
    {
        var readModel = await GetOrCreateTrackedEntity(domainEvent.UserId, cancellationToken);

        readModel.Email = domainEvent.Email;

        readModel.FullName = $"{domainEvent.FirstName} {domainEvent.LastName}"
                             + (string.IsNullOrWhiteSpace(domainEvent.MiddleName)
                                 ? string.Empty
                                 : $" {domainEvent.MiddleName}");

        readModel.ShortName = $"{domainEvent.LastName} {domainEvent.FirstName.ToUpperInvariant().First()}."
                              + (string.IsNullOrWhiteSpace(domainEvent.MiddleName)
                                  ? string.Empty
                                  : $" {domainEvent.MiddleName.ToUpperInvariant().First()}.");
    }

    private async Task When(UserPersonalInfoChanged domainEvent, CancellationToken cancellationToken)
    {
        var readModel = await GetOrCreateTrackedEntity(domainEvent.UserId, cancellationToken);

        readModel.FullName = $"{domainEvent.LastName} {domainEvent.FirstName}"
                             + (string.IsNullOrWhiteSpace(domainEvent.MiddleName)
                                 ? string.Empty
                                 : $" {domainEvent.MiddleName}");

        readModel.ShortName = $"{domainEvent.LastName} {domainEvent.FirstName.ToUpperInvariant().First()}."
                              + (string.IsNullOrWhiteSpace(domainEvent.MiddleName)
                                  ? string.Empty
                                  : $" {domainEvent.MiddleName.ToUpperInvariant().First()}.");
    }

    private async Task When(UserEmailChanged domainEvent, CancellationToken cancellationToken)
    {
        var readModel = await GetOrCreateTrackedEntity(domainEvent.UserId, cancellationToken);

        readModel.Email = domainEvent.Email;
    }

    protected override async Task<UserReadModel> GetOrCreateTrackedEntity(
        Guid entityId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext
            .Set<UserReadModel>()
            .FindAsync([entityId], cancellationToken: cancellationToken);

        if (entity == null)
        {
            entity = new UserReadModel
            {
                Id = entityId,
                Email = string.Empty,
                FullName = string.Empty,
                ShortName = string.Empty,
            };

            _dbContext.Add(entity);
        }

        return entity;
    }
}
