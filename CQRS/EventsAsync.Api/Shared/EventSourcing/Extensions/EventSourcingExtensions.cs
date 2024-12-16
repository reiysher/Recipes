using EventsAsync.Api.Shared.EventSourcing.Abstractions;
using EventsAsync.Api.Shared.EventSourcing.EfCoreStorage;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EventsAsync.Api.Shared.EventSourcing.Extensions;

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
    public void AddInlineProjection<TProjection>()
        where TProjection : class, IInlineProjection
    {
        services.AddTransient<IInlineProjection, TProjection>();
    }

    public void AddLiveProjection<TProjection>()
        where TProjection : class, ILiveProjection
    {
        services.AddTransient<ILiveProjection, TProjection>();
    }

    public void AddAsyncProjection<TProjection>()
        where TProjection : class, IAsyncProjection
    {
        services.AddTransient<TProjection>();
        services.AddHostedService<ProjectionService<TProjection>>();
    }
}
