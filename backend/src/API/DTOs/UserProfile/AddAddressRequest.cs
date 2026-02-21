namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for adding an address to a profile.
/// </summary>
public class AddAddressRequest
{
    public string Type { get; set; } = "Home";
    public AddressRequest Address { get; set; } = new();
}