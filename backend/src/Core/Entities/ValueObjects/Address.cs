namespace Core.Entities.ValueObjects;

/// <summary>
/// Value object representing a user's address (1:N relationship with UserProfile).
/// </summary>
public sealed record Address
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Line1 { get; init; } = string.Empty;
    public string? Line2 { get; init; }
    public string Suburb { get; init; } = string.Empty;
    public string StateProvince { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string Postcode { get; init; } = string.Empty;

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
