namespace StateAsync.Api.Shared.Persistence;

// todo: need better name
internal sealed class SyncEntity
{
    public required string Type { get; set; }

    public required DateTime LastSyncUtc { get; set; }

    public required bool HasChanges { get; set; }
}