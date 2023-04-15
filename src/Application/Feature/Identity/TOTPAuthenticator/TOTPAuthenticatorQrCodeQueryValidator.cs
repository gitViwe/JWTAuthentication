namespace Application.Feature.Identity.TOTPAuthenticator;

public class TOTPAuthenticatorQrCodeQueryValidator : AbstractValidator<TOTPAuthenticatorQrCodeQuery>
{
    public TOTPAuthenticatorQrCodeQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Invalid user details.");
    }
}
