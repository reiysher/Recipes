using EventsAsync.Api.Shared.Abstractions;
using EventsAsync.Api.Shared.Entities;
using EventsAsync.Api.Shared.EventSourcing.Abstractions;

namespace EventsAsync.Api.Shared.Persistence;

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