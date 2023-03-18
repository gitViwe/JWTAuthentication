using Application.Common.Interface;
using gitViwe.Shared;
using gitViwe.Shared.Extension;
using Infrastructure.Persistance.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Shared.Constant;
using Shared.Contract.Identity;
using System.Security.Claims;

namespace Infrastructure.Service;

internal class HubIdentityService : IHubIdentityService
{
    private readonly IUserClaimsPrincipalFactory<HubIdentityUser> _claimsPrincipalFactory;
    private readonly UserManager<HubIdentityUser> _userManager;
    private readonly IJWTTokenService _tokenService;
    private readonly ILogger<HubIdentityService> _logger;

    public HubIdentityService(
        IUserClaimsPrincipalFactory<HubIdentityUser> claimsPrincipalFactory,
        UserManager<HubIdentityUser> userManager,
        IJWTTokenService tokenService,
        ILogger<HubIdentityService> logger)
    {
        _claimsPrincipalFactory = claimsPrincipalFactory;
        _userManager = userManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<IResponse<TokenResponse>> LoginUserAsync(LoginRequest request, CancellationToken token)
    {
        // verify if email is registered
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            // verify that the password is valid
            if (await _userManager.CheckPasswordAsync(existingUser, request.Password))
            {
                // get claims principal from user
                var claimsPrincipal = await _claimsPrincipalFactory.CreateAsync(existingUser);
                // create JWT token
                return Response<TokenResponse>.Success("Login successful.", _tokenService.GenerateToken(claimsPrincipal));
            }
        }

        return Response<TokenResponse>.Fail("Login details invalid.", StatusCodes.Status401Unauthorized);
    }

    public Task LogoutUserAsync(string tokenId, CancellationToken token)
    {
        _tokenService.FlagAsRevokedToken(tokenId);
        return Task.CompletedTask;
    }

    public async Task<IResponse<TokenResponse>> RefreshToken(TokenRequest request, CancellationToken token)
    {
        var claimsPrincipal = _tokenService.ValidateToken(request, isRefreshToken: true);

        // mark refresh token as used
        _tokenService.FlagAsUsedToken(claimsPrincipal.GetTokenID());

        // verify if email is registered
        var existingUser = await _userManager.FindByIdAsync(claimsPrincipal.GetUserId());

        if (existingUser is not null)
        {
            // get claims principal from user
            claimsPrincipal = await _claimsPrincipalFactory.CreateAsync(existingUser);
            // create JWT token
            return Response<TokenResponse>.Success("Token refresh successful.", _tokenService.GenerateToken(claimsPrincipal));
        }

        return Response<TokenResponse>.Fail("Token refresh failed, please login again.", StatusCodes.Status401Unauthorized);
    }

    public async Task<IResponse<TokenResponse>> RegisterAsync(RegisterRequest request, CancellationToken token)
    {
        // verify if email is already registered
        if (await _userManager.FindByEmailAsync(request.Email) is not null)
        {
            return Response<TokenResponse>.Fail("This email is already registered.", StatusCodes.Status401Unauthorized);
        }
        // create identity user object
        var newUser = new HubIdentityUser { Email = request.Email, UserName = request.UserName };
        // register new user using request details
        var result = await _userManager.CreateAsync(newUser, request.Password);
        // continue if user has been created
        if (!result.Succeeded)
        {
            _logger.LogWarning("Registration attempt failed with result: {result}", result);
            return Response<TokenResponse>.Fail("Unable to create your account at this time.", StatusCodes.Status401Unauthorized);
        }
        // add required permission
        await _userManager.AddClaimAsync(newUser, new Claim(HubClaimTypes.Permission, HubPermissions.Profile.Manage));
        // get claims principal from user
        var claimsPrincipal = await _claimsPrincipalFactory.CreateAsync(newUser);
        // create JWT token
        return Response<TokenResponse>.Success("Registration successful.", _tokenService.GenerateToken(claimsPrincipal));
    }
}
