using Application.Feature.SuperHero;
using MediatR;
using Shared.Contract.SuperHero;
using Shared.Wrapper;
using System.Net;
using System.Net.Mime;

namespace API.Endpoint
{
    /// <summary>
    /// Endpoints for hub super hero sample data
    /// </summary>
    public static class SuperHeroEndpoint
    {
        /// <summary>
        /// Maps the API endpoints for the SuperHeroService
        /// </summary>
        internal static void MapSuperHeroEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet(Shared.Route.API.SuperHeroEndpoint.GetAll, GetAll)
                .AllowAnonymous()
                .WithName("GetAll")
                .Produces<PaginatedResponse<SuperHeroResponse>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json);
        }

        private static async Task<IResult> GetAll(
            int currentPage,
            int pageSize,
            IMediator mediator,
            CancellationToken token = default)
        {
            var response = await mediator.Send(new GetSuperHeroQuery { CurrentPage = currentPage, PageSize = pageSize }, token);
            return Results.Ok(response);
        }
    }
}
