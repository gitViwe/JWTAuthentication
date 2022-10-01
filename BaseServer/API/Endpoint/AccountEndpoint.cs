using Application.Feature.Identity.LoginUser;
using Application.Feature.Identity.RefreshToken;
using Application.Feature.Identity.RegisterUser;
using MediatR;
using Shared.Contract.Identity;
using Shared.Wrapper;
using System.Net;
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
        app.MapPost(Shared.Route.API.AcccountEndpoint.Register, Register)
            .AllowAnonymous()
            .WithName("Register")
            .Produces<Response<TokenResponse>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json);

        app.MapPost(Shared.Route.API.AcccountEndpoint.Login, Login)
            .AllowAnonymous()
            .WithName("Login")
            .Produces<Response<TokenResponse>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json);

        app.MapPost(Shared.Route.API.AcccountEndpoint.RefreshToken, RefreshToken)
            .AllowAnonymous()
            .WithName("RefreshToken")
            .Produces<Response<TokenResponse>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json);
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
