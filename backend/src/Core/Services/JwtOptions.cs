namespace Core.Services;

public class JwtOptions
{
    public string SigningKey { get; set; } = string.Empty;
    public int ExpireDays { get; set; } = 1;
}
