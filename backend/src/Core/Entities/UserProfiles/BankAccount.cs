using Core.Entities;

namespace Core.Entities.UserProfiles;

/// <summary>
/// Entity representing bank account details (PII - encrypted).
/// 1:N relationship with UserProfile.
/// </summary>
public class BankAccount : BaseEntity
{
    /// <summary>
    /// Bank name (e.g., "ANZ", "Westpac", "BNZ")
    /// </summary>
    public string BankName { get; set; } = string.Empty;

    /// <summary>
    /// Account type (e.g., "Savings", "Current", "Checking")
    /// </summary>
    public string AccountType { get; set; } = string.Empty;

    /// <summary>
    /// Account holder name (PII - encrypted)
    /// </summary>
    [Encrypt]
    public string AccountHolderName { get; set; } = string.Empty;

    /// <summary>
    /// Account number (PII - encrypted)
    /// </summary>
    [Encrypt]
    public string AccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// Branch code / routing number (optional)
    /// </summary>
    public string? BranchCode { get; set; }

    /// <summary>
    /// Whether this account is verified
    /// </summary>
    public bool IsVerified { get; set; }

    // For EF Core navigation
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }
}