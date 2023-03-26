namespace Application.Feature.Identity.TOTPAuthenticator;

public class TOTPAuthenticatorVerifyCommandValidator : AbstractValidator<TOTPAuthenticatorVerifyCommand>
{
    public TOTPAuthenticatorVerifyCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("Email Address is invalid");
    }
}
