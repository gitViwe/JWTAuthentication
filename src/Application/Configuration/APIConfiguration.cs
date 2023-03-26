namespace Application.Configuration;

/// <summary>
/// Maps the <see cref="APIConfiguration"/> section in AppSettings to this model
/// </summary>
public class APIConfiguration
{
    public string ApplicationName { get; set; } = string.Empty;
    public string Secret { get; init; } = string.Empty;
    public string ClientUrl { get; init; } = string.Empty;
    public string ServerUrl { get; init; } = string.Empty;
    public int TokenExpityInMinutes { get; init; }
    public int RefreshTokenExpityInMinutes { get; init; }
}
