using Application.Feature.SuperHero;
using gitViwe.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Contract.SuperHero;
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
    internal static void MapSuperHeroEndpoint(this IEndpointRouteBuilder app, IConfiguration configuration)
    {
        app.MapGet(Shared.Route.API.SuperHeroEndpoint.GetPaginated, GetPaginated)
            .WithName(nameof(GetPaginated))
            .WithTags(Shared.Route.API.SuperHeroEndpoint.TAG_NAME)
            .Produces<PaginatedResponse<SuperHeroResponse>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status401Unauthorized, "application/problem+json")
            .ProducesValidationProblem(contentType: "application/problem+json")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get a list of super hero models.",
                Description = "Gets a paginated list of super hero models based on the query parameters.",
            });
    }

    private static async Task<IResult> GetPaginated(
        [FromServices] IMediator mediator,
        [FromQuery] int CurrentPage = 1,
        [FromQuery] int PageSize = 15,
        CancellationToken token = default)
    {
        var response = await mediator.Send(new GetSuperHeroQuery { CurrentPage = CurrentPage, PageSize = PageSize }, token);
        return Results.Ok(response);
    }
}
