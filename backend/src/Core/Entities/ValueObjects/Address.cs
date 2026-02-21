namespace Core.Entities.ValueObjects;

/// <summary>
/// Value object representing a user's address (1:N relationship with UserProfile).
/// </summary>
public sealed class Address : ValueObject
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Line1 { get; set; } = string.Empty;
    public string? Line2 { get; set; }
    public string Suburb { get; set; } = string.Empty;
    public string StateProvince { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;

    // For EF Core navigation
    public Guid UserProfileId { get; set; }

    public Address() { }

    public Address(
        string line1,
        string suburb,
        string stateProvince,
        string country,
        string postcode,
        string? line2 = null
    )
    {
        Line1 = line1;
        Line2 = line2;
        Suburb = suburb;
        StateProvince = stateProvince;
        Country = country;
        Postcode = postcode;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        // Id is excluded from equality - value objects compare by value, not identity
        yield return Line1;
        yield return Line2;
        yield return Suburb;
        yield return StateProvince;
        yield return Country;
        yield return Postcode;
    }
}
