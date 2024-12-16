using EventsSync.Api.Shared.Abstractions;
using EventsSync.Api.Shared.Entities;
using EventsSync.Api.Shared.EventSourcing;
using EventsSync.Api.Shared.EventSourcing.Abstractions;

namespace EventsSync.Api.Shared.Persistence;

internal sealed class UserRepository(IEventStore eventStore) : IUserRepository
{
    public Task<User?> Get(Guid userId, CancellationToken cancellationToken)
    {
        return eventStore.AggregateStream<User>(userId, cancellationToken);
    }

    public Task Save(User user, CancellationToken cancellationToken)
    {
        return eventStore.Save(user.Id, user, cancellationToken);
    }
}