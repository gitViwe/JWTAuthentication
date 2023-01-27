namespace Application.Feature.SuperHero;

public class GetSuperHeroQueryValidator : AbstractValidator<GetSuperHeroQuery>
{
    public GetSuperHeroQueryValidator()
    {
        RuleFor(x => x.CurrentPage)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1);
    }
}
