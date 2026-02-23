namespace Core.Entities.UserProfiles;

/// <summary>
/// Entity representing an education qualification.
/// 1:N relationship with UserProfile.
/// </summary>
public class Education : BaseEntity
{
    /// <summary>
    /// Institution name (e.g., "University of Auckland", "Harvard University")
    /// </summary>
    public string Institution { get; set; } = string.Empty;

    /// <summary>
    /// Degree/qualification type (e.g., "Bachelor", "Master", "PhD", "Diploma")
    /// </summary>
    public string Degree { get; set; } = string.Empty;

    /// <summary>
    /// Field of study (e.g., "Computer Science", "Business Administration")
    /// </summary>
    public string FieldOfStudy { get; set; } = string.Empty;

    /// <summary>
    /// Start date of the program
    /// </summary>
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// End date of the program (null if currently studying)
    /// </summary>
    public DateOnly? EndDate { get; set; }

    /// <summary>
    /// Grade/GPA achieved (optional)
    /// </summary>
    public string? Grade { get; set; }

    /// <summary>
    /// Certificate or transcript reference number (optional)
    /// </summary>
    [Encrypt]
    public string? CertificateNumber { get; set; }

    /// <summary>
    /// Whether this education is verified
    /// </summary>
    public bool IsVerified { get; set; }

    // For EF Core navigation
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }
}
