namespace Core.Enums;

/// <summary>
/// Enum to indicate where to fetch the resource ID from in the HTTP request.
/// </summary>
public enum ValueFetchFrom
{
    /// <summary>
    /// Fetch from route parameter
    /// </summary>
    Route = 1,

    /// <summary>
    /// Fetch from query string
    /// </summary>
    Query = 2,

    /// <summary>
    /// Fetch from HTTP header
    /// </summary>
    Header = 3,

    /// <summary>
    /// Fetch from request body
    /// </summary>
    Body = 4
}
