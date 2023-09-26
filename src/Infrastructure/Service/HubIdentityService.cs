using Application.ApiClient;
using Infrastructure.Persistence.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Contract.Identity;

namespace Infrastructure.Service;

internal class HubIdentityService : IHubIdentityService
{
    private readonly UserManager<HubIdentityUser> _userManager;
    private readonly RoleManager<HubIdentityRole> _roleManager;
    private readonly ILogger<HubIdentityService> _logger;
    private readonly IImageHostingClient _imageHosting;
    private readonly IMongoDBRepository<HubIdentityUserData> _userDataRepository;
    private readonly HubDbContext _dbContext;
    private readonly ITimeBasedOTPService _timeBasedOTP;
    private readonly IConfiguration _configuration;
    private readonly ISecurityTokenService _tokenService;
    private readonly IHttpContextAccessor _contextAccessor;

    public HubIdentityService(
        UserManager<HubIdentityUser> userManager,
        ILogger<HubIdentityService> logger,
        IImageHostingClient imageHosting,
        IMongoDBRepository<HubIdentityUserData> userDataRepository,
        RoleManager<HubIdentityRole> roleManager,
        HubDbContext dbContext,
        ITimeBasedOTPService timeBasedOTP,
        IConfiguration configuration,
        ISecurityTokenService tokenService,
        IHttpContextAccessor contextAccessor)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _contextAccessor = contextAccessor;
        _logger = logger;
        _imageHosting = imageHosting;
        _userDataRepository = userDataRepository;
        _roleManager = roleManager;
        _dbContext = dbContext;
        _timeBasedOTP = timeBasedOTP;
        _configuration = configuration;
        _tokenService = tokenService;
    }

    public async Task<IResponse<QrCodeImageResponse>> GetQrCodeImageAsync(QrCodeImageRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId)
            ?? throw new UnauthorizedException($"User with Id: [{request.UserId}] does not exist.");

        var qrCodeData = _timeBasedOTP.GenerateQrCodeData(user.UserName!, _configuration[HubConfigurations.API.ApplicationName]!, out string secretKey);
        user.TOTPKey = secretKey;

        var response = new QrCodeImageResponse()
        {
            QrCodeImage = _timeBasedOTP.GetGraphicAsByteArray(qrCodeData),
        };

        await _userManager.UpdateAsync(user);

        return Response<QrCodeImageResponse>.Success("QrCode created.", response);
    }

    public async Task<IResponse> ValidateTOTPAsync(string userId, string totp, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new UnauthorizedException($"User with Id: [{userId}] does not exist.");

        if (string.IsNullOrWhiteSpace(user.TOTPKey))
        {
            return Response.Fail("Invalid details.");
        }

        if (_timeBasedOTP.VerifyTOTP(user.TOTPKey, totp) == false)
        {
            return Response.Fail("Invalid details.");
        }

        // get claims principal from user
        var claimsPrincipal = await CreateClaimsPrincipalAsync(user, cancellationToken);

        if (!claimsPrincipal.HasClaim(HubClaimTypes.Permission, HubPermissions.Authentication.VerifiedTOTP))
        {
            await _userManager.AddClaimAsync(user, new Claim(HubClaimTypes.Permission, HubPermissions.Authentication.VerifiedTOTP));
        }

        return Response.Success("The TOTP has been verified successfully.");
    }

    public async Task<IResponse<TokenResponse>> LoginUserAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            if (await _userManager.CheckPasswordAsync(existingUser, request.Password))
            {
                var claimsPrincipal = await CreateClaimsPrincipalAsync(existingUser, cancellationToken);

                var securityToken = _tokenService.CreateToken(claimsPrincipal.Claims, GetAudienceUrl());

                var refreshToken = CreateRefreshToken(securityToken.Id, existingUser.Id);

                var response = new TokenResponse()
                {
                    RefreshToken = refreshToken.Token,
                    Token = _tokenService.WriteToken(securityToken),
                };

                _dbContext.RefreshTokens.Add(refreshToken);
                _dbContext.SaveChanges();

                return Response<TokenResponse>.Success("Login successful.", response);
            }
        }

        return Response<TokenResponse>.Fail("Login details invalid.", StatusCodes.Status401Unauthorized);
    }

    public async Task LogoutUserAsync(string tokenId, CancellationToken cancellationToken)
    {
        var refreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.JwtId == tokenId, cancellationToken);
        if (refreshToken is not null)
        {
            refreshToken.IsRevoked = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IResponse<TokenResponse>> RefreshToken(TokenRequest request, CancellationToken cancellationToken)
    {
        var claimsPrincipal = _tokenService.ValidateToken(request.Token, isRefreshToken: true);

        // mark refresh token as used
        var refreshToken = await ValidateRefreshTokenAsync(claimsPrincipal, request.RefreshToken, cancellationToken);
        refreshToken.IsRevoked = true;
        await _dbContext.SaveChangesAsync(cancellationToken);

        var existingUser = await _userManager.FindByIdAsync(claimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

        if (existingUser is not null)
        {
            claimsPrincipal = await CreateClaimsPrincipalAsync(existingUser, cancellationToken);

            var securityToken = _tokenService.CreateToken(claimsPrincipal.Claims, GetAudienceUrl());

            var newRefreshToken = CreateRefreshToken(securityToken.Id, existingUser.Id);

            var response = new TokenResponse()
            {
                RefreshToken = newRefreshToken.Token,
                Token = _tokenService.WriteToken(securityToken),
            };

            _dbContext.RefreshTokens.Add(newRefreshToken);
            _dbContext.SaveChanges();

            return Response<TokenResponse>.Success("Token refresh successful.", response);
        }

        return Response<TokenResponse>.Fail("Token refresh failed, please login again.", StatusCodes.Status401Unauthorized);
    }

    public async Task<IResponse<TokenResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not null)
        {
            return Response<TokenResponse>.Fail("This email is already registered.", StatusCodes.Status401Unauthorized);
        }

        var newUser = new HubIdentityUser { Email = request.Email, UserName = request.UserName };

        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Registration attempt failed with result: {result}", result);
            return Response<TokenResponse>.Fail("Unable to create your account at this time.", StatusCodes.Status401Unauthorized);
        }

        await _userManager.AddClaimAsync(newUser, new Claim(HubClaimTypes.Permission, HubPermissions.Profile.Manage));

        var claimsPrincipal = await CreateClaimsPrincipalAsync(newUser, cancellationToken);

        var securityToken = _tokenService.CreateToken(claimsPrincipal.Claims, GetAudienceUrl());

        var refreshToken = CreateRefreshToken(securityToken.Id, newUser.Id);

        var response = new TokenResponse()
        {
            RefreshToken = refreshToken.Token,
            Token = _tokenService.WriteToken(securityToken),
        };

        _dbContext.RefreshTokens.Add(refreshToken);
        _dbContext.SaveChanges();

        return Response<TokenResponse>.Success("Registration successful.", response);
    }

    public async Task<IResponse> UpdateUserAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new UnauthorizedException($"User with Id: [{userId}] does not exist.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;

        await UpdateUserAsync(user);

        return Response.Success("User details updated.");
    }

    public async Task<IResponse> UploadImageAsync(string userId, UploadImageRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new UnauthorizedException($"User with Id: [{userId}] does not exist.");

        var uploadResult = await _imageHosting.UploadImageAsync(request.File);
        var userData = new HubIdentityUserData { ProfileImage = uploadResult, };

        await _userDataRepository.ReplaceOneAsync(userData, cancellationToken);

        user.HubIdentityUserDataId = userData.Id.ToString();

        await UpdateUserAsync(user);

        return Response.Success("User details updated.");
    }

    public async Task<IResponse<UserDetailResponse>> GetUserDetailAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new UnauthorizedException($"User with Id: [{userId}] does not exist.");

        var userDetail = new UserDetailResponse()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            Username = user.UserName ?? string.Empty,
        };

        if (!string.IsNullOrEmpty(user.HubIdentityUserDataId))
        {
            var userData = await _userDataRepository.FindByIdAsync(user.HubIdentityUserDataId, cancellationToken);

            if (userData is not null)
            {
                userDetail.ProfileImage = new()
                {
                    Image = userData.ProfileImage.Data.Image,
                    Thumbnail = userData.ProfileImage.Data.Thumb,
                };
            }
        }

        return Response<UserDetailResponse>.Success("User details retrieved.", userDetail);
    }

    private async Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(HubIdentityUser user, CancellationToken cancellationToken)
    {
        List<Claim> claims = new()
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        };

        // get claims that are assigned to the user...
        var userClaims = await _userManager.GetClaimsAsync(user);
        if (userClaims is not null && userClaims.Any())
        {
            claims.AddRange(userClaims);
        }

        // get roles that are assigned to the user
        foreach (var role in await GetRolesAsync(user, cancellationToken))
        {
            if (role is null)
            {
                // skip this iteration
                continue;
            }

            if (!string.IsNullOrWhiteSpace(role.Name))
            {
                // add the role to the claims collection
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            // get all claims associated with that role
            foreach (var roleClaim in await _roleManager.GetClaimsAsync(role))
            {
                claims.Add(roleClaim);
            }
        }

        async Task<IEnumerable<HubIdentityRole>> GetRolesAsync(HubIdentityUser user, CancellationToken cancellationToken)
        {
            // force the enumerable to execute rather than joining
            var userRoleIds = await _dbContext.UserRoles
                                            .AsNoTracking()
                                            .Where(x => x.UserId == user.Id)
                                            .Select(x => x.RoleId)
                                            .ToArrayAsync(cancellationToken);

            if (userRoleIds is not null && userRoleIds.Any())
            {
                var roles = await _dbContext.Roles
                                            .AsNoTracking()
                                            .Where(x => userRoleIds.Contains(x.Id))
                                            .ToArrayAsync(cancellationToken);

                return roles is not null && roles.Any() ? roles : Enumerable.Empty<HubIdentityRole>();
            }

            return Enumerable.Empty<HubIdentityRole>();
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims));
    }

    private async Task UpdateUserAsync(HubIdentityUser user)
    {
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var sb = new StringBuilder()
                .AppendLine("Unable to update user details.")
                .AppendLine(string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)));

            throw new UnauthorizedException(sb.ToString(), "Unable to update user details.");
        }
    }

    private async Task<RefreshToken> ValidateRefreshTokenAsync(ClaimsPrincipal claimsPrincipal, string refreshToken, CancellationToken cancellationToken)
    {
        var storedToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(item => item.Token == refreshToken, cancellationToken)
            ?? throw new UnauthorizedException("The token does not exist.");

        if (storedToken.IsUsed)
        {
            throw new UnauthorizedException("The token has already been used.");
        }

        if (storedToken.IsRevoked)
        {
            throw new UnauthorizedException("The token has been revoked.");
        }

        if (storedToken.ExpiryDate < DateTime.UtcNow)
        {
            throw new UnauthorizedException("The token has expired.");
        }

        var tokenID = claimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.Jti)!;
        if (storedToken.JwtId != tokenID)
        {
            throw new UnauthorizedException($"The token with ID: {tokenID}, is not valid.");
        }

        return storedToken;
    }

    private RefreshToken CreateRefreshToken(string jwtId, string userId)
    {
        return new RefreshToken()
        {
            JwtId = jwtId,
            IsUsed = false,
            IsRevoked = false,
            UserId = userId,
            AddedDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMinutes(int.Parse(_configuration[HubConfigurations.API.RefreshTokenExpityInMinutes]!)),
            Token = Generator.RandomString(CharacterCombination.NumberAndAlphabet, 65)
        };
    }

    private string GetAudienceUrl()
    {
        if (_contextAccessor.HttpContext is null)
        {
            throw new UnauthorizedException("Invalid request host.");
        }

        var values = _contextAccessor.HttpContext.Request.Headers.Origin;

        return values.FirstOrDefault() ?? throw new UnauthorizedException("Invalid request host.");
    }
}
