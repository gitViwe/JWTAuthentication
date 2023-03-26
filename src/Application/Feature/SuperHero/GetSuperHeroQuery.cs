using Shared.Contract.SuperHero;

namespace Application.Feature.SuperHero;

public class GetSuperHeroQuery : PaginatedRequest, IRequest<PaginatedResponse<SuperHeroResponse>> { }

internal class GetSuperHeroQueryHandler : IRequestHandler<GetSuperHeroQuery, PaginatedResponse<SuperHeroResponse>>
{
    private readonly ISuperHeroService _heroService;

    public GetSuperHeroQueryHandler(ISuperHeroService heroService)
    {
        _heroService = heroService;
    }

    public Task<PaginatedResponse<SuperHeroResponse>> Handle(GetSuperHeroQuery request, CancellationToken cancellationToken)
    {
        return _heroService.GetPaginatedAsync(request, cancellationToken);
    }
}
