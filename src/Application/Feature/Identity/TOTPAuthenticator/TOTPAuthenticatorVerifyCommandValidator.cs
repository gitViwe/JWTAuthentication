namespace Application.Feature.Identity.TOTPAuthenticator;

public class TOTPAuthenticatorVerifyCommandValidator : AbstractValidator<TOTPAuthenticatorVerifyCommand>
{
    public TOTPAuthenticatorVerifyCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Invalid user details.");
    }
}
