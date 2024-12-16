namespace EventsAsync.Api.Shared.Abstractions;

public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    public TId Id { get; protected set; } = default!;

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        return left is not null && right is not null && left.Equals(right) || left is null && right is null;
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !(left == right);
    }

    public bool Equals(Entity<TId>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (other is not Entity<TId> entity)
        {
            return false;
        }

        return other.Id.Equals(Id);
    }

    public override bool Equals(object? obj)
    {
        if(obj == null)
        {
            return false;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        if(obj is not Entity<TId> entity)
        {
            return false;
        }

        return entity.Id.Equals(Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
