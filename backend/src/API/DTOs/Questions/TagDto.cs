namespace API.DTOs.Questions;

/// <summary>
/// DTO for a tag.
/// </summary>
public class TagDto
{
    /// <summary>
    /// Tag name
    /// </summary>
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for a tag (includes ID).
/// </summary>
public class TagResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}