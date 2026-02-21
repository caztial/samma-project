namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for address value object.
/// </summary>
public class AddressRequest
{
    public string Line1 { get; set; } = string.Empty;
    public string? Line2 { get; set; }
    public string Suburb { get; set; } = string.Empty;
    public string StateProvince { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for UserAddress entity.
/// </summary>
public class AddressResponse
{
    public Guid Id { get; set; }
    public string Type { get; set; } = "Home";
    public string Line1 { get; set; } = string.Empty;
    public string? Line2 { get; set; }
    public string Suburb { get; set; } = string.Empty;
    public string StateProvince { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
}