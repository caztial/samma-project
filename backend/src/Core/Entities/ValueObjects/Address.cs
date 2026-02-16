namespace Core.Entities.ValueObjects;

/// <summary>
/// Value object representing a user's address (1:N relationship with UserProfile).
/// </summary>
public sealed class Address
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
}
