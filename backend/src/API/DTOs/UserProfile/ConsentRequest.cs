namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for adding a consent.
/// </summary>
public class ConsentRequest
{
    public string TermId { get; set; } = string.Empty;
    public string TermLink { get; set; } = string.Empty;
    public string TermsVersion { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for consent.
/// </summary>
public class ConsentResponse
{
    public Guid Id { get; set; }
    public string TermId { get; set; } = string.Empty;
    public string TermLink { get; set; } = string.Empty;
    public string TermsVersion { get; set; } = string.Empty;
    public DateTime AcceptedAt { get; set; }
    public string IpAddress { get; set; } = string.Empty;
}
