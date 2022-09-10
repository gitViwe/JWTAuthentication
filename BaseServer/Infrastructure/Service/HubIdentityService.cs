using Application.Common.Interface;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Shared.Constant;
using Shared.Contract.Identity;
using Shared.Extension;
using Shared.Wrapper;
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

    public async Task<IResponse> LoginUserAsync(LoginRequest request, CancellationToken token)
    {
        // verify if email is registered
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        // no matching email
        if (existingUser is not null)
        {
            // verify that the password is valid
            if (await _userManager.CheckPasswordAsync(existingUser, request.Password))
            {
                // get claims principal from user
                var claimsPrincipal = await _claimsPrincipalFactory.CreateAsync(existingUser);
                // create JWT token
                var newToken = _tokenService.GenerateToken(claimsPrincipal);
                return Response<TokenResponse>.Success("Login successful", newToken);
            }
        }
        
        return Response.Fail("Login details invalid.");
    }

    public IResponse RefreshToken(TokenRequest request)
    {
        var claimsPrincipal = _tokenService.ValidateToken(request);

        if (claimsPrincipal.HasExpiredClaims())
        {
            _tokenService.FlagAsUsedToken(claimsPrincipal.GetTokenID());
        }

        var newToken = _tokenService.GenerateToken(claimsPrincipal);
        return Response<TokenResponse>.Success("Token refresh successful.", newToken);
    }

    public async Task<IResponse> RegisterAsync(RegisterRequest request, CancellationToken token)
    {
        // verify if email is already registered
        if (await _userManager.FindByEmailAsync(request.Email) is not null)
        {
            return Response.Fail("This email is already registered.");
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
            var newToken = _tokenService.GenerateToken(claimsPrincipal);
            return Response<TokenResponse>.Success("Registration successful.", newToken);
        }
        // errors that occurred during user registration
        return Response.Fail(result.Errors.Select(error => error.Description));
    }
}
