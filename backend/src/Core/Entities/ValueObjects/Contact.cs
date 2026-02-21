using Core.Entities;

namespace Core.Entities.ValueObjects;

/// <summary>
/// Value object representing contact information (PII - encrypted).
/// </summary>
public sealed class Contact : ValueObject
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

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ContactNumber;
        yield return Email;
    }
}
