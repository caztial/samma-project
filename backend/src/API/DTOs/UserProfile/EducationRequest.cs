namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for education data.
/// </summary>
public class EducationRequest
{
    public string Institution { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Grade { get; set; }
    public string? CertificateNumber { get; set; }
    public bool IsVerified { get; set; }
}

/// <summary>
/// Response DTO for education.
/// </summary>
public class EducationResponse
{
    public Guid Id { get; set; }
    public string Institution { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Grade { get; set; }
    public string? CertificateNumber { get; set; }
    public bool IsVerified { get; set; }
}
