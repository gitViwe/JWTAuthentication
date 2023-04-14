namespace Application.Feature.Identity.UserDetail;

public class UserDetailQueryValidator : AbstractValidator<UserDetailQuery>
{
    public UserDetailQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
