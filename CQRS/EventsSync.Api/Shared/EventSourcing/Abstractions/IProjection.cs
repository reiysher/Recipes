using EventsSync.Api.Shared.Abstractions;

namespace EventsSync.Api.Shared.EventSourcing.Abstractions;

public interface IBaseProjection
{
    bool CanHandle(Type eventType);

    Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken);
}

public interface IInlineProjection : IBaseProjection;
public interface ILiveProjection : IBaseProjection;
public interface IAsyncProjection : IBaseProjection;

public interface IProjection : IInlineProjection, ILiveProjection, IAsyncProjection;

public abstract class BaseProjection<TReadModel> : IProjection
    where TReadModel : class
{
    private readonly Dictionary<Type, Func<IDomainEvent, CancellationToken, Task>> _handlers = [];

    public bool CanHandle(Type eventType)
    {
        return _handlers.ContainsKey(eventType);
    }

    protected void Projects<TEvent>(Func<TEvent, CancellationToken, Task> handler)
        where TEvent : IDomainEvent
    {
        _handlers.Add(typeof(TEvent), (domainEvent, cancellationToken) => handler((TEvent)domainEvent, cancellationToken));
    }

    protected abstract Task<TReadModel> GetOrCreateTrackedEntity(Guid entityId, CancellationToken cancellationToken = default);

    public async Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await _handlers[domainEvent.GetType()](domainEvent, cancellationToken);
    }
}
