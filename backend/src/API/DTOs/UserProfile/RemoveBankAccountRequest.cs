namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for removing a bank account.
/// </summary>
public class RemoveBankAccountRequest
{
    public Guid Id { get; set; }
    public Guid BankAccountId { get; set; }
}
