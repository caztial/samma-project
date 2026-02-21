namespace Core.Entities.ValueObjects;

/// <summary>
/// Value object representing address details - compared by value.
/// </summary>
public sealed class Address : ValueObject
{
    public string Line1 { get; } = string.Empty;
    public string? Line2 { get; }
    public string Suburb { get; } = string.Empty;
    public string StateProvince { get; } = string.Empty;
    public string Country { get; } = string.Empty;
    public string Postcode { get; } = string.Empty;

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

    public static Address Empty => new();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Line1;
        yield return Line2;
        yield return Suburb;
        yield return StateProvince;
        yield return Country;
        yield return Postcode;
    }
}