namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for adding a bank account to a profile.
/// </summary>
public class AddBankAccountRequest
{
    public BankAccountRequest BankAccount { get; set; } = new();
}
