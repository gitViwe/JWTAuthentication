using Application.Common.Interface;
using Application.Configuration;
using Infrastructure.Identity;
using Infrastructure.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Constant;
using Shared.Contract.Identity;
using Shared.Exception;
using Shared.Extension;
using Shared.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Service;

internal class JWTTokenService : IJWTTokenService
{
    private readonly APIConfiguration _configuration;
    private readonly HubDbContext _dbContext;
    private readonly TokenValidationParameters _validationParameters;
    private readonly ILogger<JWTTokenService> _logger;

    public JWTTokenService(
        IOptionsMonitor<APIConfiguration> optionsMonitor,
        HubDbContext dbContext,
        TokenValidationParameters validationParameters,
        ILogger<JWTTokenService> logger)
    {
        _configuration = optionsMonitor.CurrentValue;
        _dbContext = dbContext;
        _validationParameters = validationParameters;
        _logger = logger;
    }

    public TokenResponse GenerateToken(ClaimsPrincipal claimsPrincipal)
    {
        // create token handler object
        var handler = new JwtSecurityTokenHandler();
        // encode security key
        var key = Encoding.ASCII.GetBytes(_configuration.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Audience = _configuration.ClientUrl,
            Issuer = _configuration.ServerUrl,
            Subject = new ClaimsIdentity(GetRequiredClaims(claimsPrincipal.Claims)),
            Expires = DateTime.UtcNow.AddMinutes(10),
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
            ExpiryDate = DateTime.UtcNow.AddMinutes(120),
            Token = Conversion.RandomString(65)
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

    public ClaimsPrincipal ValidateToken(TokenRequest request)
    {
        var handler = new JwtSecurityTokenHandler();
        try
        {
            // verify that the token is a JWT token
            var jwtClaims = handler.ValidateToken(request.Token, _validationParameters, out var validatedToken);

            if (validatedToken is JwtSecurityToken securityToken)
            {
                // verify that the token is encrypted with the security algorithm
                var result = securityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                if (result is false)
                {
                    _logger.LogError("Token has invalid security algorithm.");
                    throw new HubIdentityException();
                }
            }

            // verify that the JWT token is on the database
            var storedToken = _dbContext.RefreshTokens.FirstOrDefault(item => item.Token == request.RefreshToken);
            if (storedToken is null)
            {
                _logger.LogError("The token does not exist.");
                throw new HubIdentityException();
            }

            // verify that the token is not being used
            if (storedToken.IsUsed)
            {
                _logger.LogError("The token has already been used.");
                throw new HubIdentityException();
            }

            // verify that the token has not been revoked
            if (storedToken.IsRevoked)
            {
                _logger.LogError("The token has been revoked.");
                throw new HubIdentityException();
            }

            // verify the token ID
            var tokenID = jwtClaims?.Claims?.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (storedToken.JwtId != tokenID)
            {
                _logger.LogError($"The token with ID: {tokenID}, is not valid.");
                throw new HubIdentityException();
            }

            return jwtClaims;
        }
        catch (SecurityTokenExpiredException ex)
        {
            _logger.LogError("Token has already expired.", ex);
            throw new HubIdentityException();
        }
    }

    public void FlagAsUsedToken(string JwtId)
    {
        // verify that the JWT token is on the database
        var storedToken = _dbContext.RefreshTokens.FirstOrDefault(item => item.JwtId == JwtId);
        if (storedToken is null)
        {
            _logger.LogError("The token does not exist.");
            throw new HubIdentityException();
        }
        // update current token and save changes
        storedToken.IsUsed = true;
        _dbContext.RefreshTokens.Update(storedToken);
        _dbContext.SaveChanges();
    }

    public void FlagAsRevokedToken(string JwtId)
    {
        // verify that the JWT token is on the database
        var storedToken = _dbContext.RefreshTokens.FirstOrDefault(item => item.JwtId == JwtId);
        if (storedToken is null)
        {
            _logger.LogError("The token does not exist.");
            throw new HubIdentityException();
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
}
