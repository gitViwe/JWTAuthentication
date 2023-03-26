namespace Application.Feature.Identity.TOTPAuthenticator;

public class TOTPAuthenticatorQrCodeQueryValidator : AbstractValidator<TOTPAuthenticatorQrCodeQuery>
{
    public TOTPAuthenticatorQrCodeQueryValidator()
    {
        RuleFor(x => x.UserEmail).NotEmpty().WithMessage("Invalid user details.");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Invalid user details.");
    }
}
