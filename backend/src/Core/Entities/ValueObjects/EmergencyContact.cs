using Core.Entities;

namespace Core.Entities.ValueObjects;

/// <summary>
/// Value object representing emergency contact information (PII - encrypted).
/// 1:N relationship with UserProfile.
/// </summary>
public sealed class EmergencyContact : ValueObject
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    [Encrypt]
    public string ContactNumber { get; set; } = string.Empty;

    public string Relationship { get; set; } = string.Empty;

    [Encrypt]
    public string Email { get; set; } = string.Empty;

    // For EF Core navigation
    public Guid UserProfileId { get; set; }

    public EmergencyContact() { }

    public EmergencyContact(string name, string contactNumber, string relationship, string email)
    {
        Name = name;
        ContactNumber = contactNumber;
        Relationship = relationship;
        Email = email;
    }

    public static EmergencyContact Empty =>
        new(string.Empty, string.Empty, string.Empty, string.Empty);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        // Id is excluded from equality - value objects compare by value, not identity
        yield return Name;
        yield return ContactNumber;
        yield return Relationship;
        yield return Email;
    }
}
