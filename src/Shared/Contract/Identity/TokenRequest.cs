using gitViwe.Shared;

namespace Shared.Contract.Identity;

public class TokenRequest : ITokenRequest
{
    public string Token { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
}
