namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for updating a bank account.
/// </summary>
public class UpdateBankAccountRequest
{
    public BankAccountRequest BankAccount { get; set; } = new();
}

/// <summary>
/// Request for updating bank account route parameters.
/// </summary>
public class UpdateBankAccountEndpointRequest
{
    public Guid Id { get; set; }
    public Guid BankAccountId { get; set; }
}
