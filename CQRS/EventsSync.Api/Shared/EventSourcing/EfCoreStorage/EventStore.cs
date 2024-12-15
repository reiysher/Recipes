using System.Text.Json;
using EventsSync.Api.Shared.Abstractions;
using EventsSync.Api.Shared.EventSourcing.Abstractions;
using EventsSync.Api.Shared.EventSourcing.Entities;
using EventsSync.Api.Shared.EventSourcing.Exceptions;
using EventsSync.Api.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventsSync.Api.Shared.EventSourcing.EfCoreStorage;

internal sealed class EventStore(
    ApplicationDbContext dbContext,
    TimeProvider timeProvider,
    IEnumerable<IInlineProjection> inlineProjections,
    IEnumerable<ILiveProjection> liveProjections)
    : IEventStore
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> GetNewDomainEvents() => _domainEvents;

    public Task<T?> AggregateStream<T>(Guid streamId, CancellationToken cancellationToken)
        where T : class, IAggregateRoot
    {
        return AggregateStream<T>(streamId, null, null, cancellationToken);
    }

    public async Task<bool> Save<TStream>(Guid streamId, TStream aggregate, CancellationToken cancellationToken)
        where TStream : IAggregateRoot
    {
        var domainEvents = aggregate.DomainEvents;
        var initialVersion = aggregate.Version - domainEvents.Count;

        foreach (var domainEvent in domainEvents)
        {
            await AppendEvent<TStream>(streamId, domainEvent, initialVersion++, cancellationToken);
        }

        _domainEvents.AddRange(domainEvents);

        return true;
    }

    private async Task AppendEvent<TStream>(Guid streamId, IDomainEvent domainEvent, long? expectedVersion = null, CancellationToken cancellationToken = default)
        where TStream : notnull
    {
        var stream = await GetStreamById(streamId, cancellationToken);
        stream ??= CreateStream(streamId, typeof(TStream).AssemblyQualifiedName!);

        if (expectedVersion.HasValue && stream.Version != expectedVersion)
        {
            throw new EventStoreConcurrencyException(streamId);
        }

        var eventPayloadJson = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), _jsonOptions);

        var eventData = new EventData
        {
            Id = Guid.NewGuid(),
            Type = domainEvent.GetType().AssemblyQualifiedName!,
            Payload = eventPayloadJson,
            Created = timeProvider.GetUtcNow(),
            StreamId = streamId,
            Version = stream.Version + 1
        };

        stream.Version = eventData.Version;

        dbContext.Set<EventData>().Add(eventData);
    }


    private async Task<T?> AggregateStream<T>(
        Guid streamId,
        long? atStreamVersion,
        DateTimeOffset? atTimestamp,
        CancellationToken cancellationToken)
        where T : class, IAggregateRoot
    {
        var events = await GetEvents(streamId, atStreamVersion, atTimestamp, cancellationToken);

        if (events.Count == 0)
        {
            return null;
        }

        var aggregate = (T)Activator.CreateInstance(typeof(T), true)!;
        var version = 0;

        foreach (var @event in events)
        {
            aggregate.Apply(@event);
            aggregate.Version = ++version;
        }

        return aggregate;
    }

    private async Task<List<IDomainEvent>> GetEvents(
        Guid streamId,
        long? atStreamVersion = null,
        DateTimeOffset? atTimestamp = null,
        CancellationToken cancellationToken = default)
    {
        var events = await dbContext
            .Set<EventData>()
            .Where(e => e.StreamId == streamId)
            .Where(e => !atStreamVersion.HasValue || e.Version <= atStreamVersion)
            .Where(e => !atTimestamp.HasValue || e.Created <= atTimestamp)
            .OrderBy(e => e.Version)
            .ToListAsync(cancellationToken);

        var deserializedEvents = new List<IDomainEvent>();

        foreach (var eventData in events)
        {
            var eventType = Type.GetType(eventData.Type);

            if (eventType == null)
            {
                throw new InvalidOperationException($"Event type {eventData.Type} not found.");
            }

            var eventPayload = JsonSerializer.Deserialize(eventData.Payload, eventType, _jsonOptions) as IDomainEvent;

            if (eventPayload is not null)
            {
                deserializedEvents.Add(eventPayload);
            }
        }

        return deserializedEvents;
    }

    private EventStream CreateStream(Guid streamId, string streamName)
    {
        var stream = new EventStream
        {
            Id = streamId,
            Type = streamName,
            Version = 0,
            Events = []
        };

        dbContext.Set<EventStream>().Add(stream);

        return stream;
    }

    private async Task<EventStream?> GetStreamById(Guid streamId, CancellationToken cancellationToken)
    {
        return await dbContext
            .Set<EventStream>()
            .FirstOrDefaultAsync(s => s.Id == streamId, cancellationToken);
    }
}
