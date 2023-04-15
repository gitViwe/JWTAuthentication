namespace Application.Feature.Identity.UpdateUser;

public class UpdateUserRequestCommandValidator : AbstractValidator<UpdateUserRequestCommand>
{
    public UpdateUserRequestCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Invalid user details.");

        RuleFor(x => x.FirstName)
            .NotEmpty();

        RuleFor(x => x.LastName)
            .NotEmpty();
    }
}
