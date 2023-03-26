using gitViwe.Shared;

namespace Shared.Contract.Identity;

public record TokenResponse
{
    public string Token { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
}
