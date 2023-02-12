using gitViwe.Shared;

namespace Shared.Contract.Identity;

public class LoginRequest : ILoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
