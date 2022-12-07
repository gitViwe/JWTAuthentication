namespace Application.Feature.Identity.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
