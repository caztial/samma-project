using Core.Entities.Questions;
using Core.Enums;

namespace Core.Entities.Sessions;

/// <summary>
/// Session Aggregate Root - represents a Dhamma session where participants can join
/// and answer questions in real-time.
/// </summary>
public class Session : BaseEntity, IAggregatedRoot
{
    /// <summary>
    /// The name of the session.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Unique code for participants to join (e.g., "Xwer-wetr-were").
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// Optional location where the session takes place.
    /// </summary>
    public string? Location { get; private set; }

    /// <summary>
    /// The current state of the session.
    /// </summary>
    public SessionState State { get; private set; } = SessionState.Draft;

    /// <summary>
    /// When the session was activated (started).
    /// </summary>
    public DateTime? StartedAt { get; private set; }

    /// <summary>
    /// When the session was ended.
    /// </summary>
    public DateTime? EndedAt { get; private set; }

    /// <summary>
    /// User ID who created this session.
    /// </summary>
    public string CreatedBy { get; private set; } = string.Empty;

    /// <summary>
    /// Participants who have joined this session.
    /// </summary>
    public virtual ICollection<SessionParticipant> Participants { get; protected set; } = [];

    /// <summary>
    /// Questions assigned to this session.
    /// </summary>
    public virtual ICollection<SessionQuestion> SessionQuestions { get; protected set; } = [];

    // EF Core parameterless constructor
    protected Session() { }

    /// <summary>
    /// Creates a new session with the specified details.
    /// </summary>
    public Session(string name, string code, string? location, string createdBy)
    {
        Name = name;
        Code = code;
        Location = location;
        CreatedBy = createdBy;
        State = SessionState.Draft;
    }

    /// <summary>
    /// Activates the session, allowing participants to join.
    /// </summary>
    public void Activate()
    {
        if (State == SessionState.Ended)
            throw new InvalidOperationException("Cannot activate an ended session.");

        State = SessionState.Active;
        StartedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates (pauses) the session.
    /// </summary>
    public void Deactivate()
    {
        if (State != SessionState.Active)
            throw new InvalidOperationException("Can only deactivate an active session.");

        State = SessionState.Inactive;
    }

    /// <summary>
    /// Reactivates a paused session.
    /// </summary>
    public void Reactivate()
    {
        if (State != SessionState.Inactive)
            throw new InvalidOperationException("Can only reactivate an inactive session.");

        State = SessionState.Active;
    }

    /// <summary>
    /// Ends the session permanently.
    /// </summary>
    public void End()
    {
        if (State == SessionState.Ended)
            throw new InvalidOperationException("Session is already ended.");

        State = SessionState.Ended;
        EndedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if participants can join this session.
    /// </summary>
    public bool CanJoin()
    {
        return State == SessionState.Active;
    }

    /// <summary>
    /// Updates session details.
    /// </summary>
    public void Update(string name, string? location)
    {
        Name = name;
        Location = location;
        UpdatedAt = DateTime.UtcNow;
    }
}
