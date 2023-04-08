namespace Application.Feature.Identity.UpdateUser;

internal class UpdateUserRequestCommandValidator : AbstractValidator<UpdateUserRequestCommand>
{
    public UpdateUserRequestCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("Email Address is invalid");

        RuleFor(x => x.FirstName)
            .NotEmpty();

        RuleFor(x => x.LastName)
            .NotEmpty();
    }
}
