namespace Core.Entities.ValueObjects;

/// <summary>
/// Value object representing user consent to terms (1:N relationship with UserProfile).
/// </summary>
public sealed class Consent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TermId { get; set; } = string.Empty;
    public string TermLink { get; set; } = string.Empty;
    public string TermsVersion { get; set; } = string.Empty;
    public DateTime AcceptedAt { get; init; }
    public string IpAddress { get; init; } = string.Empty;

    // For EF Core navigation
    public Guid UserProfileId { get; set; }

    public Consent() { }

    public Consent(string termId, string termLink, string termsVersion, string ipAddress)
    {
        TermId = termId;
        TermLink = termLink;
        TermsVersion = termsVersion;
        AcceptedAt = DateTime.UtcNow;
        IpAddress = ipAddress;
    }
}
