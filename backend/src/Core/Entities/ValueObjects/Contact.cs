using Core.Entities;

namespace Core.Entities.ValueObjects;

/// <summary>
/// Value object representing contact information (PII - encrypted).
/// </summary>
public sealed class Contact
{
    [Encrypt]
    public string ContactNumber { get; private set; } = string.Empty;
    
    [Encrypt]
    public string Email { get; private set; } = string.Empty;

    public Contact() { }

    public Contact(string contactNumber, string email)
    {
        ContactNumber = contactNumber;
        Email = email;
    }

    public static Contact Empty => new(string.Empty, string.Empty);

    public override bool Equals(object? obj)
    {
        if (obj is not Contact other) return false;
        return ContactNumber == other.ContactNumber && Email == other.Email;
    }

    public override int GetHashCode() => HashCode.Combine(ContactNumber, Email);

    public static bool operator ==(Contact? left, Contact? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Contact? left, Contact? right) => !(left == right);
}
