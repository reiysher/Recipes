using StateSync.Api.Shared.Abstractions;

namespace StateSync.Api.Shared.Persistence;

public interface ISyncProjectionHandler
{
    bool CanHandle(IAggregateRoot aggregate);

    Task Handle(IAggregateRoot aggregate);
}
