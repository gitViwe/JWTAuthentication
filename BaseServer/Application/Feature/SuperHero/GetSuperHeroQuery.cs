using Application.Common.Interface;
using Shared.Contract.SuperHero;

namespace Application.Feature.SuperHero
{
    public class GetSuperHeroQuery : PaginatedRequest, IRequest<PaginatedResponse<SuperHeroResponse>> { }

    public class GetSuperHeroQueryHandler : IRequestHandler<GetSuperHeroQuery, PaginatedResponse<SuperHeroResponse>>
    {
        private readonly ISuperHeroService _heroService;

        public GetSuperHeroQueryHandler(ISuperHeroService heroService)
        {
            _heroService = heroService;
        }

        public async Task<PaginatedResponse<SuperHeroResponse>> Handle(GetSuperHeroQuery request, CancellationToken cancellationToken)
        {
            return await _heroService.GetPaginatedAsync(request, cancellationToken);
        }
    }
}
