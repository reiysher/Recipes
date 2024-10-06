using StateAsync.Api.Shared.Abstractions;

namespace StateAsync.Api.Shared.Persistence;

public interface IAsyncProjectionHandler
{
    bool CanHandle(IProjectionSource aggregate);

    Task Handle(IProjectionSource aggregate);
}
