using StateSync.Api.Shared.Abstractions;

namespace StateSync.Api.Shared.Persistence;

public interface IProjectionHandler
{
    bool CanHandle(IAggregateRoot aggregate);

    Task Handle(IAggregateRoot aggregate);
}
