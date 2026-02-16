using Core.Entities;

namespace Core.Entities.ValueObjects;

/// <summary>
/// Value object representing emergency contact information (PII - encrypted).
/// 1:N relationship with UserProfile.
/// </summary>
public sealed class EmergencyContact
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    [Encrypt]
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

    public override bool Equals(object? obj)
    {
        if (obj is not EmergencyContact other)
            return false;
        return Name == other.Name
            && ContactNumber == other.ContactNumber
            && Relationship == other.Relationship
            && Email == other.Email;
    }

    public override int GetHashCode() => HashCode.Combine(Name, ContactNumber, Relationship, Email);

    public static bool operator ==(EmergencyContact? left, EmergencyContact? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(EmergencyContact? left, EmergencyContact? right) =>
        !(left == right);
}
