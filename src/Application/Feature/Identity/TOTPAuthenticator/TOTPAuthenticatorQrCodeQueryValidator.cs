namespace Application.Feature.Identity.TOTPAuthenticator;

public class TOTPAuthenticatorQrCodeQueryValidator : AbstractValidator<TOTPAuthenticatorQrCodeQuery>
{
    public TOTPAuthenticatorQrCodeQueryValidator()
    {
        RuleFor(x => x.UserEmail).NotEmpty().WithMessage(x => $"Invalid user details. [{x}]");
        RuleFor(x => x.UserId).NotEmpty().WithMessage(x => $"Invalid user details. [{x}]");
    }
}
