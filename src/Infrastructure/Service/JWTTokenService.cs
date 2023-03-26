using Application.Common.Interface;
using Application.Configuration;
using gitViwe.Shared;
using gitViwe.Shared.Extension;
using Infrastructure.Identity;
using Infrastructure.Persistance;
using Infrastructure.Persistance.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Constant;
using Shared.Contract.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Service;

internal class JWTTokenService : IJWTTokenService
{
    private readonly APIConfiguration _configuration;
    private readonly HubDbContext _dbContext;
    private readonly TokenValidationParameters _validationParameters;
    private readonly RefreshTokenValidationParameters _refreshValidationParameters;
    private readonly IHttpContextAccessor _contextAccessor;

    public JWTTokenService(
        IOptionsMonitor<APIConfiguration> optionsMonitor,
        HubDbContext dbContext,
        TokenValidationParameters validationParameters,
        RefreshTokenValidationParameters refreshValidationParameters,
        IHttpContextAccessor contextAccessor)
    {
        _configuration = optionsMonitor.CurrentValue;
        _dbContext = dbContext;
        _validationParameters = validationParameters;
        _refreshValidationParameters = refreshValidationParameters;
        _contextAccessor = contextAccessor;
    }

    public TokenResponse GenerateToken(ClaimsPrincipal claimsPrincipal)
    {
        // create token handler object
        var handler = new JwtSecurityTokenHandler();
        // encode security key
        var key = Encoding.ASCII.GetBytes(_configuration.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Audience = GetAudienceUrl(),
            Issuer = _configuration.ServerUrl,
            Subject = new ClaimsIdentity(GetRequiredClaims(claimsPrincipal.Claims)),
            Expires = DateTime.UtcNow.AddMinutes(_configuration.TokenExpityInMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        // creates a security token
        var token = handler.CreateToken(tokenDescriptor);
        // serialize the security token
        var jwtToken = handler.WriteToken(token);
        // create refresh token entity
        var refreshToken = new RefreshToken()
        {
            JwtId = token.Id,
            IsUsed = false,
            IsRevoked = false,
            UserId = claimsPrincipal.GetUserId(),
            AddedDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMinutes(_configuration.RefreshTokenExpityInMinutes),
            Token = Generator.RandomString(length: 65)
        };
        // save to database
        _dbContext.RefreshTokens.Add(refreshToken);
        _dbContext.SaveChanges();
        // return the authentication result
        return new TokenResponse()
        {
            Token = jwtToken,
            RefreshToken = refreshToken.Token,
        };
    }

    public ClaimsPrincipal ValidateToken(TokenRequest request, bool isRefreshToken = false)
    {
        var handler = new JwtSecurityTokenHandler();
        try
        {
            // verify that the token is a JWT token
            var jwtClaims = isRefreshToken
                ? handler.ValidateToken(request.Token, _refreshValidationParameters, out SecurityToken validatedToken)
                : handler.ValidateToken(request.Token, _validationParameters, out validatedToken);

            if (jwtClaims is null)
            {
                throw new UnauthorizedException("The token is invalid.");
            }

            if (validatedToken is JwtSecurityToken securityToken)
            {
                // verify that the token is encrypted with the security algorithm
                var result = securityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                if (result is false)
                {
                    throw new UnauthorizedException("Token has invalid security algorithm.");
                }
            }

            // verify that the JWT token is on the database
            var storedToken = _dbContext.RefreshTokens.FirstOrDefault(item => item.Token == request.RefreshToken);
            if (storedToken is null)
            {
                throw new UnauthorizedException("The token does not exist.");
            }

            // verify that the token is not being used
            if (storedToken.IsUsed)
            {
                throw new UnauthorizedException("The token has already been used.");
            }

            // verify that the token has not been revoked
            if (storedToken.IsRevoked)
            {
                throw new UnauthorizedException("The token has been revoked.");
            }

            // verify the token ID
            var tokenID = jwtClaims.GetTokenID();
            if (storedToken.JwtId != tokenID)
            {
                throw new UnauthorizedException($"The token with ID: {tokenID}, is not valid.");
            }

            return jwtClaims;
        }
        catch (SecurityTokenValidationException ex)
        {
            throw new UnauthorizedException("Token validation failed.", ex);
        }
    }

    public void FlagAsUsedToken(string JwtId)
    {
        // verify that the JWT token is on the database
        var storedToken = _dbContext.RefreshTokens.FirstOrDefault(item => item.JwtId == JwtId && !item.IsUsed);
        if (storedToken is null)
        {
            throw new UnauthorizedException("The token does not exist.");
        }
        // update current token and save changes
        storedToken.IsUsed = true;
        _dbContext.RefreshTokens.Update(storedToken);
        _dbContext.SaveChanges();
    }

    public void FlagAsRevokedToken(string JwtId)
    {
        // verify that the JWT token is on the database
        var storedToken = _dbContext.RefreshTokens.FirstOrDefault(item => item.JwtId == JwtId && !item.IsRevoked);
        if (storedToken is null)
        {
            throw new UnauthorizedException("The token does not exist.");
        }
        // update current token and save changes
        storedToken.IsRevoked = true;
        _dbContext.RefreshTokens.Update(storedToken);
        _dbContext.SaveChanges();
    }

    private static IEnumerable<Claim> GetRequiredClaims(IEnumerable<Claim> claims)
    {
        IEnumerable<string> requiredClaims = new string[]
        {
            ClaimTypes.NameIdentifier,
            ClaimTypes.Name,
            ClaimTypes.Email,
            HubClaimTypes.Permission
        };

        var userClaims = claims.Where(x => requiredClaims.Contains(x.Type)).ToList();
        userClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

        return userClaims;
    }

    private string GetAudienceUrl()
    {
        if (_contextAccessor.HttpContext is null)
        {
            throw new UnauthorizedException("Invalid request host.");
        }

        string requestHost = _contextAccessor.HttpContext.Request.Host.Value;
        string protocolScheme = _contextAccessor.HttpContext.Request.Scheme;
        return $"{protocolScheme}{Uri.SchemeDelimiter}{requestHost}";
    }
}
