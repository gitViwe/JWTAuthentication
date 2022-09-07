using Application.Common.Interface;
using Application.Configuration;
using Infrastructure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Contract.Identity;
using Shared.Extension;
using Shared.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Service;

internal class JWTTokenService : IJWTTokenService
{
    private readonly APIConfiguration _configuration;

    public JWTTokenService(IOptionsMonitor<APIConfiguration> optionsMonitor)
    {
        _configuration = optionsMonitor.CurrentValue;
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
            Subject = new ClaimsIdentity(claimsPrincipal.Claims),
            Expires = DateTime.UtcNow.AddMinutes(5),
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
            UserId = new Guid(claimsPrincipal.GetUserId()),
            AddedDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMinutes(120),
            Token = Conversion.RandomString(35) + Guid.NewGuid()
        };
        // return the authentication result
        return new TokenResponse()
        {
            Token = jwtToken,
            RefreshToken = refreshToken.Token,
        };
    }
}
