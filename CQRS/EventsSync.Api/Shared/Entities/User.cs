using EventsSync.Api.Shared.Abstractions;

namespace EventsSync.Api.Shared.Entities;

public sealed class User : AggregateRoot<Guid>
{
    private User()
    {
        // to satisfy ef core
    }

    public User(Guid id, string firstName, string lastName, string? middleName, string email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        Email = email;

        var domainEvent = new UserCreated(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            Id,
            FirstName,
            LastName,
            MiddleName,
            Email);

        RaiseDomainEvent(domainEvent);
    }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string? MiddleName { get; private set; }

    public string Email { get; private set; }

    public void ChangePersonalInfo(string firstName, string lastName, string? middleName)
    {
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;

        var domainEvent = new UserPersonalInfoChanged(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            Id,
            FirstName,
            LastName,
            MiddleName);

        RaiseDomainEvent(domainEvent);
    }

    public void ChangeEmail(string email)
    {
        Email = email;

        var domainEvent = new UserEmailChanged(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            Id,
            Email);

        RaiseDomainEvent(domainEvent);
    }

    public override void Apply(IDomainEvent domainEvent)
    {
        When(domainEvent as dynamic);
    }

    private void When(UserCreated domainEvent)
    {
        Id = domainEvent.UserId;
        FirstName = domainEvent.FirstName;
        LastName = domainEvent.LastName;
        MiddleName = domainEvent.MiddleName;
        Email = domainEvent.Email;
    }

    private void When(UserPersonalInfoChanged domainEvent)
    {
        FirstName = domainEvent.FirstName;
        LastName = domainEvent.LastName;
        MiddleName = domainEvent.MiddleName;
    }

    private void When(UserEmailChanged domainEvent)
    {
        Email = domainEvent.Email;
    }
}