using EventsSync.Api.Shared.EventSourcing.Abstractions;
using EventsSync.Api.Shared.EventSourcing.EfCoreStorage;
using EventsSync.Api.Shared.EventSourcing.Enums;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EventsSync.Api.Shared.EventSourcing.Extensions;

public static class EventSourcingExtensions
{
    public static IServiceCollection AddEventSourcing(
        this IServiceCollection services,
        Action<EventSourcingOptions>? options = null)
    {
        services.TryAddScoped<IEventStore, EventStore>();

        options?.Invoke(new EventSourcingOptions(services));

        return services;
    }
}

public sealed class EventSourcingOptions(IServiceCollection services)
{
    public void AddProjection<TProjection>(ProjectionLifecycle lifecycle)
        where TProjection : class, IProjection
    {
        switch (lifecycle)
        {
            case ProjectionLifecycle.Inline:
                services.AddTransient<IInlineProjection, TProjection>();
                break;
            case ProjectionLifecycle.Async:
                services.AddTransient<IAsyncProjection, TProjection>();
                break;
            case ProjectionLifecycle.Live:
                services.AddTransient<ILiveProjection, TProjection>();
                break;
        }
    }
}
