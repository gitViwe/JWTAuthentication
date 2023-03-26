namespace Shared.Contract.Identity;

public class TokenRequest
{
    public string Token { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
}
