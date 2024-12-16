using EventsAsync.Api.Shared.Abstractions;

namespace EventsAsync.Api.Shared.EventSourcing.Abstractions;

public interface IBaseProjection
{
    IReadOnlyCollection<Type> RelevantEventTypes { get; }

    Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken);
}

public interface IInlineProjection : IBaseProjection;

public interface ILiveProjection : IBaseProjection;

public interface IAsyncProjection : IBaseProjection
{
    int BatchSize { get; }

    int Delay { get; }
}

public interface IProjection;

public abstract class BaseProjection<TReadModel> : IProjection
    where TReadModel : class
{
    private readonly Dictionary<Type, Func<IDomainEvent, CancellationToken, Task>> _handlers = [];

    public IReadOnlyCollection<Type> RelevantEventTypes => _handlers.Keys;

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
