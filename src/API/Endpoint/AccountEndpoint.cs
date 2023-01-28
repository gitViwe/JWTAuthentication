using Application.Feature.Identity.LoginUser;
using Application.Feature.Identity.RefreshToken;
using Application.Feature.Identity.RegisterUser;
using MediatR;
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
        var endpointGroup = app.MapGroup(Shared.Route.API.AcccountEndpoint.PREFIX)
            .AllowAnonymous()
            .WithTags(Shared.Route.API.AcccountEndpoint.TAG_NAME);

        endpointGroup.MapPost(Shared.Route.API.AcccountEndpoint.Register, Register)
            .WithName(nameof(Register))
            .Produces<TokenResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();

        endpointGroup.MapPost(Shared.Route.API.AcccountEndpoint.Login, Login)
            .WithName(nameof(Login))
            .Produces<TokenResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();

        endpointGroup.MapPost(Shared.Route.API.AcccountEndpoint.RefreshToken, RefreshToken)
            .WithName(nameof(RefreshToken))
            .Produces<TokenResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> Register(
        RegisterRequest request,
        IMediator mediator,
        CancellationToken token = default)
    {
        var command = new RegisterUserCommand
        {
            Email = request.Email,
            Password = request.Password,
            PasswordConfirmation = request.PasswordConfirmation,
            UserName = request.UserName
        };
        return Results.Ok(await mediator.Send(command, token));
    }

    private static async Task<IResult> Login(
        LoginRequest request,
        IMediator mediator,
        CancellationToken token = default)
    {
        var command = new LoginUserCommand
        {
            Email = request.Email,
            Password = request.Password,
        };
        return Results.Ok(await mediator.Send(command, token));
    }

    private static async Task<IResult> RefreshToken(
        TokenRequest request,
        IMediator mediator,
        CancellationToken token = default)
    {
        var command = new RefreshTokenCommand()
        {
            RefreshToken = request.RefreshToken,
            Token = request.Token,
        };
        return Results.Ok(await mediator.Send(command, token));
    }
}
