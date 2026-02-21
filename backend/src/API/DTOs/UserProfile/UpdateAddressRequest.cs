namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for updating an address.
/// </summary>
public class UpdateAddressRequest
{
    public string Type { get; set; } = "Home";
    public AddressRequest Address { get; set; } = new();
}

/// <summary>
/// Request for updating address route parameters.
/// </summary>
public class UpdateAddressEndpointRequest
{
    public Guid Id { get; set; }
    public Guid AddressId { get; set; }
}