using StateAsync.Api.Shared.Abstractions;

namespace StateAsync.Api.Shared.Entities;

public sealed class User : IProjectionSource
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
        Created = DateTime.UtcNow;
        Updated = DateTime.UtcNow;
    }

    public Guid Id { get; private init; }


    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string? MiddleName { get; private set; }

    public string Email { get; private set; }

    public DateTime Created { get; private set; }

    public DateTime? Updated { get; private set; }

    public void ChangePersonalInfo(string firstName, string lastName, string? middleName)
    {
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        Updated = DateTime.UtcNow;
    }

    public void ChangeEmail(string email)
    {
        Email = email;
        Updated = DateTime.UtcNow;
    }

    internal string GetFullName() =>
        $"{LastName} {FirstName}"
        + (string.IsNullOrWhiteSpace(MiddleName) ? string.Empty : $" {MiddleName}");

    internal string GetShortName() =>
        $"{LastName} {FirstName.ToUpperInvariant().First()}."
        + (string.IsNullOrWhiteSpace(MiddleName) ? string.Empty : $" {MiddleName.ToUpperInvariant().First()}.");
}