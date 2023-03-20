using Application.Feature.Identity.LoginUser;
using Application.Feature.Identity.LogoutUser;
using Application.Feature.Identity.RefreshToken;
using Application.Feature.Identity.RegisterUser;
using gitViwe.ProblemDetail;
using gitViwe.Shared.Extension;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Contract.Identity;
using System.Net.Mime;

namespace API.Endpoint;

/// <summary>
/// Endpoint for Hub user accounts
/// </summary>
public static class AccountEndpoint
{
    /// <summary>
    /// Maps the API endpoints for the Account
    /// </summary>
    internal static void MapAccountEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(Shared.Route.API.AcccountEndpoint.Register, RegisterAsync)
            .AllowAnonymous()
            .WithName(nameof(RegisterAsync))
            .WithTags(Shared.Route.API.AcccountEndpoint.TAG_NAME)
            .Produces<TokenResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status401Unauthorized, "application/problem+json")
            .ProducesValidationProblem(contentType: "application/problem+json")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Register a new user.",
                Description = "Provide a new email and password to create the account.",
            });

        app.MapPost(Shared.Route.API.AcccountEndpoint.Login, LoginAsync)
            .AllowAnonymous()
            .WithName(nameof(LoginAsync))
            .WithTags(Shared.Route.API.AcccountEndpoint.TAG_NAME)
            .Produces<TokenResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status401Unauthorized, "application/problem+json")
            .ProducesValidationProblem(contentType: "application/problem+json")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Login an existing user.",
                Description = "Provide an email and password to get a JSON web token for this user.",
            });

        app.MapPost(Shared.Route.API.AcccountEndpoint.RefreshToken, RefreshTokenAsync)
            .AllowAnonymous()
            .WithName(nameof(RefreshTokenAsync))
            .WithTags(Shared.Route.API.AcccountEndpoint.TAG_NAME)
            .Produces<TokenResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status401Unauthorized, "application/problem+json")
            .ProducesValidationProblem(contentType: "application/problem+json")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Refresh JSON web token for an existing user.",
                Description = "Provide a token and refresh token to get a new JSON web token for this user.",
            });

        app.MapPost(Shared.Route.API.AcccountEndpoint.Logout, LogoutAsync)
            .WithName(nameof(LogoutAsync))
            .WithTags(Shared.Route.API.AcccountEndpoint.TAG_NAME)
            .ProducesProblem(StatusCodes.Status401Unauthorized, "application/problem+json")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Invalidate JSON web token for an existing user.",
                Description = "Flags the new JSON web token for this user as used and revoked.",
            });
    }

    private static async Task<IResult> RegisterAsync(
        RegisterRequest request,
        [FromServices] IHttpContextAccessor accessor,
        [FromServices] IMediator mediator,
        CancellationToken token = default)
    {
        var response = await mediator.Send(new RegisterUserCommand
        {
            Email = request.Email,
            Password = request.Password,
            PasswordConfirmation = request.PasswordConfirmation,
            UserName = request.UserName
        }, token);

        return response.Succeeded()
            ? Results.Ok(response.Data)
            : Results.Problem(ProblemDetailFactory.CreateProblemDetails(accessor.HttpContext!, response.StatusCode, response.Message));
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        [FromServices] IHttpContextAccessor accessor,
        [FromServices] IMediator mediator,
        CancellationToken token = default)
    {
        var response = await mediator.Send(new LoginUserCommand
        {
            Email = request.Email,
            Password = request.Password,
        }, token);

        return response.Succeeded()
            ? Results.Ok(response.Data)
            : Results.Problem(ProblemDetailFactory.CreateProblemDetails(accessor.HttpContext!, response.StatusCode, response.Message));
    }

    private static async Task<IResult> RefreshTokenAsync(
        TokenRequest request,
        [FromServices] IHttpContextAccessor accessor,
        [FromServices] IMediator mediator,
        CancellationToken token = default)
    {
        var response = await mediator.Send(new RefreshTokenCommand()
        {
            RefreshToken = request.RefreshToken,
            Token = request.Token,
        }, token);

        return response.Succeeded()
            ? Results.Ok(response.Data)
            : Results.Problem(ProblemDetailFactory.CreateProblemDetails(accessor.HttpContext!, response.StatusCode, response.Message));
    }

    private static async Task<IResult> LogoutAsync(
        [FromServices] IHttpContextAccessor accessor,
        [FromServices] IMediator mediator,
        CancellationToken token = default)
    {
        string tokenId = accessor.HttpContext?.User.GetTokenID()!;
        await mediator.Send(new LogoutCommand(tokenId), token);
        return Results.Ok();
    }
}
