namespace Shared.Contract.Identity;

public class QrCodeImageRequest
{
    public string UserId { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
}
