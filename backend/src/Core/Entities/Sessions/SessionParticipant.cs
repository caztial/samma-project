using Core.Entities;

namespace Core.Entities.Sessions;

/// <summary>
/// Represents a participant who has joined a session.
/// </summary>
public class SessionParticipant : BaseEntity
{
    /// <summary>
    /// The session this participant belongs to.
    /// </summary>
    public Guid SessionId { get; private set; }

    /// <summary>
    /// The user ID of the participant.
    /// </summary>
    public string UserId { get; private set; } = string.Empty;

    /// <summary>
    /// When the participant joined the session.
    /// </summary>
    public DateTime JoinedAt { get; private set; }

    /// <summary>
    /// When the participant left the session (null if still active).
    /// </summary>
    public DateTime? LeftAt { get; private set; }

    // Navigation properties
    public Session Session { get; private set; } = null!;
    public ApplicationUser User { get; private set; } = null!;

    /// <summary>
    /// Answers submitted by this participant.
    /// </summary>
    public virtual ICollection<ParticipantAnswer> Answers { get; private set; } = [];

    // EF Core parameterless constructor
    private SessionParticipant() { }

    /// <summary>
    /// Creates a new session participant.
    /// </summary>
    public SessionParticipant(Guid sessionId, string userId)
    {
        SessionId = sessionId;
        UserId = userId;
        JoinedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the participant as having left the session.
    /// </summary>
    public void Leave()
    {
        LeftAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if the participant is currently active in the session.
    /// </summary>
    public bool IsActiveInSession()
    {
        return LeftAt == null;
    }
}