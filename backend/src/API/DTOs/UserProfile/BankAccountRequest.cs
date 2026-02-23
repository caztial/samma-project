namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for bank account data.
/// </summary>
public class BankAccountRequest
{
    public string BankName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string AccountHolderName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string? BranchCode { get; set; }
    public bool IsVerified { get; set; }
}

/// <summary>
/// Response DTO for bank account.
/// </summary>
public class BankAccountResponse
{
    public Guid Id { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string AccountHolderName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string? BranchCode { get; set; }
    public bool IsVerified { get; set; }
}
