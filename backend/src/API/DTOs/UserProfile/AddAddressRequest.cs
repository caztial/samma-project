namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for adding an address to a profile.
/// </summary>
public class AddAddressRequest
{
    public AddressRequest Address { get; set; } = new();
}
