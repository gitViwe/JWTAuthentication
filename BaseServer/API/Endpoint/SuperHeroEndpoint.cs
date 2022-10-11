using API.Extension;
using Application.Feature.SuperHero;
using MediatR;
using Shared.Contract.SuperHero;
using Shared.Wrapper;
using System.Net;
using System.Net.Mime;

namespace API.Endpoint;

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
        app.MapGet(Shared.Route.API.SuperHeroEndpoint.GetPaginated, GetPaginated)
            .WithName(nameof(GetPaginated))
            .Produces<PaginatedResponse<SuperHeroResponse>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .WithTags(Shared.Route.API.SuperHeroEndpoint.TAG_NAME);
    }

    private static async Task<IResult> GetPaginated(
        int currentPage,
        int pageSize,
        IMediator mediator,
        CancellationToken token = default)
    {
        var response = await mediator.Send(new GetSuperHeroQuery { CurrentPage = currentPage, PageSize = pageSize }, token);
        return Results.Ok(response);
    }
}
