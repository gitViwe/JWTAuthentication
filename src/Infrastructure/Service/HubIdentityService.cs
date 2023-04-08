using Application.Common.Interface;
using Application.Configuration;
using gitViwe.Shared;
using gitViwe.Shared.Extension;
using Infrastructure.Persistance.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OtpNet;
using Shared.Constant;
using Shared.Contract.Identity;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Service;

internal class HubIdentityService : IHubIdentityService
{
    private readonly IUserClaimsPrincipalFactory<HubIdentityUser> _claimsPrincipalFactory;
    private readonly UserManager<HubIdentityUser> _userManager;
    private readonly IJWTTokenService _tokenService;
    private readonly ITOTPService _tOTPService;
    private readonly APIConfiguration _configuration;
    private readonly ILogger<HubIdentityService> _logger;

    public HubIdentityService(
        IUserClaimsPrincipalFactory<HubIdentityUser> claimsPrincipalFactory,
        UserManager<HubIdentityUser> userManager,
        IJWTTokenService tokenService,
        ITOTPService tOTPService,
        IOptionsMonitor<APIConfiguration> optionsMonitor,
        ILogger<HubIdentityService> logger)
    {
        _claimsPrincipalFactory = claimsPrincipalFactory;
        _userManager = userManager;
        _tokenService = tokenService;
        _tOTPService = tOTPService;
        _configuration = optionsMonitor.CurrentValue;
        _logger = logger;
    }

    public async Task<IResponse<QrCodeImageResponse>> GetQrCodeImageAsync(QrCodeImageRequest request, CancellationToken token)
    {
        var user = await _userManager.FindByIdAsync(request.UserId)
            ?? throw new UnauthorizedException($"User with Id: [{request.UserId}] does not exist.");

        // Generate a random secret key for the user.
        var secretKey = KeyGeneration.GenerateRandomKey(32);
        user.TOTPKey = Base32Encoding.ToString(secretKey);
        await _userManager.UpdateAsync(user);

        var response = new QrCodeImageResponse() { QrCodeImage = _tOTPService.GenerateQrCode(request.UserEmail, secretKey, _configuration.ApplicationName) };

        return Response<QrCodeImageResponse>.Success("QrCode created.", response);
    }

    public async Task<IResponse> ValidateTOTPAsync(string email, string totp, CancellationToken token)
    {
        var user = await _userManager.FindByEmailAsync(email)
            ?? throw new UnauthorizedException($"User with Email: [{email}] does not exist.");

        if (string.IsNullOrWhiteSpace(user.TOTPKey))
        {
            return Response.Fail("Invalid details.");
        }

        if (!_tOTPService.VerifyTOTP(Base32Encoding.ToBytes(user.TOTPKey), totp))
        {
            return Response.Fail("Invalid details.");
        }

        // get claims principal from user
        var claimsPrincipal = await _claimsPrincipalFactory.CreateAsync(user);

        if (!claimsPrincipal.HasClaim(HubClaimTypes.Permission, HubPermissions.Authentication.TOTP))
        {
            await _userManager.AddClaimAsync(user, new Claim(HubClaimTypes.Permission, HubPermissions.Authentication.TOTP));
        }

        return Response.Success("The TOTP has been verified successfully.");
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

    public async Task<IResponse<TokenResponse>> UpdateUserAsync(string email, UpdateUserRequest request, CancellationToken token)
    {
        var user = await _userManager.FindByEmailAsync(email)
            ?? throw new UnauthorizedException($"User with Email: [{email}] does not exist.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var sb = new StringBuilder()
                .AppendLine("Unable to update user details.")
                .AppendLine(string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)));

            throw new UnauthorizedException(sb.ToString(), "Unable to update user details.");
        }

        // get claims principal from user
        var claimsPrincipal = await _claimsPrincipalFactory.CreateAsync(user);
        // create JWT token
        return Response<TokenResponse>.Success("User details updated.", _tokenService.GenerateToken(claimsPrincipal));
    }
}
