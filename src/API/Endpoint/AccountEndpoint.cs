using Amazon.Runtime.Internal;
using Application.Feature.Identity.LoginUser;
using Application.Feature.Identity.LogoutUser;
using Application.Feature.Identity.RefreshToken;
using Application.Feature.Identity.RegisterUser;
using Application.Feature.Identity.TOTPAuthenticator;
using Application.Feature.Identity.UpdateUser;
using Application.Feature.Identity.UploadImage;
using Application.Feature.Identity.UserDetail;
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
                Summary = "Register new user.",
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
                Summary = "Login existing user.",
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
                Summary = "Refresh JWT for existing user.",
                Description = "Provide a token and refresh token to get a new JSON web token for this user.",
            });

        app.MapPost(Shared.Route.API.AcccountEndpoint.Logout, LogoutAsync)
            .WithName(nameof(LogoutAsync))
            .WithTags(Shared.Route.API.AcccountEndpoint.TAG_NAME)
            .Produces(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status401Unauthorized, "application/problem+json")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Invalidate JWT.",
                Description = "Flags the new JSON web token for this user as used and revoked.",
            });

        app.MapGet(Shared.Route.API.AcccountEndpoint.QrCode, GetQrCodeAsync)
            .WithName(nameof(GetQrCodeAsync))
            .WithTags(Shared.Route.API.AcccountEndpoint.TAG_NAME)
            .Produces(StatusCodes.Status200OK, responseType: typeof(byte[]), contentType: "image/png")
            .ProducesProblem(StatusCodes.Status401Unauthorized, "application/problem+json")
            .ProducesValidationProblem(contentType: "application/problem+json")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get QRCode.",
                Description = "Get a QrCode image to scan and set up time-based one-time password (TOTP).",
            });

        app.MapPost(Shared.Route.API.AcccountEndpoint.TOTPVerify, VerifyTOTPAsync)
            .WithName(nameof(VerifyTOTPAsync))
            .WithTags(Shared.Route.API.AcccountEndpoint.TAG_NAME)
            .Produces(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status401Unauthorized, "application/problem+json")
            .ProducesValidationProblem(contentType: "application/problem+json")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Verify TOTP.",
                Description = "Verify the time-based one-time password (TOTP).",
            });

        app.MapPut(Shared.Route.API.AcccountEndpoint.Detail, UpdateDetailsAsync)
            .WithName(nameof(UpdateDetailsAsync))
            .WithTags(Shared.Route.API.AcccountEndpoint.TAG_NAME)
            .Produces<TokenResponse>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status401Unauthorized, "application/problem+json")
            .ProducesValidationProblem(contentType: "application/problem+json")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Update user details.",
                Description = "Update the current user's first name and last name.",
            });

        app.MapPost(Shared.Route.API.AcccountEndpoint.Picture, UploadImageAsync)
            .WithName(nameof(UploadImageAsync))
            .WithTags(Shared.Route.API.AcccountEndpoint.TAG_NAME)
            .Produces<TokenResponse>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status401Unauthorized, "application/problem+json")
            .ProducesValidationProblem(contentType: "application/problem+json")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Upload profile picture.",
                Description = "Upload or replace the user's profile picture image.",
            });

        app.MapGet(Shared.Route.API.AcccountEndpoint.Detail, GetUserDetailAsync)
            .WithName(nameof(GetUserDetailAsync))
            .WithTags(Shared.Route.API.AcccountEndpoint.TAG_NAME)
            .Produces<UserDetailResponse>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status401Unauthorized, "application/problem+json")
            .ProducesValidationProblem(contentType: "application/problem+json")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get user details.",
                Description = "Get the current user's details.",
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

        return response.Succeeded
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

        return response.Succeeded
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

        return response.Succeeded
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

    private static async Task<IResult> GetQrCodeAsync(
        [FromServices] IHttpContextAccessor accessor,
        [FromServices] IMediator mediator,
        CancellationToken token = default)
    {
        var response = await mediator.Send(new TOTPAuthenticatorQrCodeQuery()
        {
            UserId = accessor.HttpContext?.User.GetUserId()!,
        }, token);

        return response.Succeeded
            ? Results.File(response.Data.QrCodeImage, contentType: "image/png", fileDownloadName: "QrCodeImage")
            : Results.Problem(ProblemDetailFactory.CreateProblemDetails(accessor.HttpContext!, response.StatusCode, response.Message));
    }

    private static async Task<IResult> VerifyTOTPAsync(
        TOTPVerifyRequest request,
        [FromServices] IHttpContextAccessor accessor,
        [FromServices] IMediator mediator,
        CancellationToken token = default)
    {
        var response = await mediator.Send(new TOTPAuthenticatorVerifyCommand()
        {
            Email = accessor.HttpContext?.User.GetEmail()!,
            Token = request.Token
        }, token);

        return response.Succeeded
            ? Results.Ok()
            : Results.Problem(ProblemDetailFactory.CreateProblemDetails(accessor.HttpContext!, response.StatusCode, response.Message));
    }

    private static async Task<IResult> UpdateDetailsAsync(
        UpdateUserRequest request,
        [FromServices] IHttpContextAccessor accessor,
        [FromServices] IMediator mediator,
        CancellationToken token = default)
    {
        var response = await mediator.Send(new UpdateUserRequestCommand()
        {
            Email = accessor.HttpContext?.User.GetEmail()!,
            FirstName = request.FirstName,
            LastName = request.LastName,
        }, token);

        return response.Succeeded
            ? Results.Ok(response.Data)
            : Results.Problem(ProblemDetailFactory.CreateProblemDetails(accessor.HttpContext!, response.StatusCode, response.Message));
    }

    private static async Task<IResult> UploadImageAsync(
        IFormFile file,
        [FromServices] IHttpContextAccessor accessor,
        [FromServices] IMediator mediator,
        CancellationToken token = default)
    {
        var response = await mediator.Send(new UploadImageRequestCommand()
        {
            UserId = accessor.HttpContext?.User.GetUserId()!,
            File = file,
        }, token);

        return response.Succeeded
            ? Results.Ok(response.Data)
            : Results.Problem(ProblemDetailFactory.CreateProblemDetails(accessor.HttpContext!, response.StatusCode, response.Message));
    }

    private static async Task<IResult> GetUserDetailAsync(
        [FromServices] IHttpContextAccessor accessor,
        [FromServices] IMediator mediator,
        CancellationToken token = default)
    {
        var response = await mediator.Send(new UserDetailQuery()
        {
            UserId = accessor.HttpContext?.User.GetUserId()!,
        }, token);

        return response.Succeeded
            ? Results.Ok(response.Data)
            : Results.Problem(ProblemDetailFactory.CreateProblemDetails(accessor.HttpContext!, response.StatusCode, response.Message));
    }
}
