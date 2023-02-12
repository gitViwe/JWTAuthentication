using Application.Common.Interface;
using gitViwe.Shared;
using gitViwe.Shared.Extension;
using Infrastructure.Persistance.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.Constant;
using Shared.Contract.Identity;
using System.Security.Claims;

namespace Infrastructure.Service;

internal class HubIdentityService : IHubIdentityService
{
    private readonly IUserClaimsPrincipalFactory<HubIdentityUser> _claimsPrincipalFactory;
    private readonly UserManager<HubIdentityUser> _userManager;
    private readonly IJWTTokenService _tokenService;

    public HubIdentityService(
        IUserClaimsPrincipalFactory<HubIdentityUser> claimsPrincipalFactory,
        UserManager<HubIdentityUser> userManager,
        IJWTTokenService tokenService)
    {
        _claimsPrincipalFactory = claimsPrincipalFactory;
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<ITokenResponse> LoginUserAsync(ILoginRequest request, CancellationToken token)
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
                return _tokenService.GenerateToken(claimsPrincipal);
            }
        }

        throw new UnauthorizedException($"Login attempt failed for email {request.Email}", "Login details invalid.");
    }

    public Task LogoutUserAsync(string tokenId, CancellationToken token)
    {
        _tokenService.FlagAsRevokedToken(tokenId);
        return Task.CompletedTask;
    }

    public async Task<ITokenResponse> RefreshToken(ITokenRequest request, CancellationToken token)
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
            return _tokenService.GenerateToken(claimsPrincipal);
        }

        throw new UnauthorizedException($"Token refresh failed for tokenId: {claimsPrincipal.GetTokenID()}", "Token refresh failed, please login again.");
    }

    public async Task<ITokenResponse> RegisterAsync(IRegisterRequest request, CancellationToken token)
    {
        // verify if email is already registered
        if (await _userManager.FindByEmailAsync(request.Email) is not null)
        {
            throw new UnauthorizedException($"Registration attempt with existing email: {request.Email}", "This email is already registered.");
        }
        // create identity user object
        var newUser = new HubIdentityUser { Email = request.Email, UserName = request.UserName };
        // register new user using request details
        var result = await _userManager.CreateAsync(newUser, request.Password);
        // continue if user has been created
        if (result.Succeeded)
        {
            // add required permission
            await _userManager.AddClaimAsync(newUser, new Claim(HubClaimTypes.Permission, HubPermissions.Profile.Manage));
            // get claims principal from user
            var claimsPrincipal = await _claimsPrincipalFactory.CreateAsync(newUser);
            // create JWT token
            return _tokenService.GenerateToken(claimsPrincipal);
        }
        // errors that occurred during user registration
        throw new UnauthorizedException($"Registration attempt failed with result: {result}", "Unable to create your account at this time.");
    }
}
