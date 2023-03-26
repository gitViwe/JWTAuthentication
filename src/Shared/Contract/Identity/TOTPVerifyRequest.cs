namespace Shared.Contract.Identity;

public class TOTPVerifyRequest
{
    public string Token { get; set; } = string.Empty;
}
