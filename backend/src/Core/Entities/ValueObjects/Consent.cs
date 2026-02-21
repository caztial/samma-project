namespace Core.Entities.ValueObjects;

/// <summary>
/// Value object representing consent details - compared by value.
/// </summary>
public sealed class Consent : ValueObject
{
    public string TermId { get; } = string.Empty;
    public string TermLink { get; } = string.Empty;
    public string TermsVersion { get; } = string.Empty;
    public DateTime AcceptedAt { get; }
    public string IpAddress { get; } = string.Empty;

    public Consent() { }

    public Consent(string termId, string termLink, string termsVersion, string ipAddress)
    {
        TermId = termId;
        TermLink = termLink;
        TermsVersion = termsVersion;
        AcceptedAt = DateTime.UtcNow;
        IpAddress = ipAddress;
    }

    public static Consent Empty => new();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return TermId;
        yield return TermLink;
        yield return TermsVersion;
        yield return AcceptedAt;
        yield return IpAddress;
    }
}