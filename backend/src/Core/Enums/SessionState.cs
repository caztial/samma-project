namespace Core.Enums;

/// <summary>
/// Represents the state of a Session in its lifecycle.
/// </summary>
public enum SessionState
{
    /// <summary>
    /// Session is in setup phase, not yet joinable.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Session is active - participants can join, questions can be presented.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Session is paused - can be resumed to Active.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Session has ended - final state, no more interactions.
    /// </summary>
    Ended = 3
}
