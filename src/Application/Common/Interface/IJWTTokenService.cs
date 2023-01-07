using Shared.Contract.Identity;
using System.Security.Claims;

namespace Application.Common.Interface;

/// <summary>
/// Helper service to facilitate JWT token processing
/// </summary>
public interface IJWTTokenService
{
    /// <summary>
    /// Creates a JWT token and refresh token
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal of the current user</param>
    /// <returns>A <see cref="TokenResponse"/> with the JWT token and refresh token</returns>
    TokenResponse GenerateToken(ClaimsPrincipal claimsPrincipal);

    /// <summary>
    /// Validates the JWT token and refresh token
    /// </summary>
    /// <param name="request">The token and refresh token to validate</param>
    /// <param name="isRefreshToken">Determines if the request is for a refresh token</param>
    /// <returns>The <see cref="ClaimsPrincipal"/> from the validate token</returns>
    /// <exception cref="Shared.Exception.HubIdentityException"></exception>
    ClaimsPrincipal ValidateToken(TokenRequest request, bool isRefreshToken = false);

    /// <summary>
    /// Updated the used flag of the refresh token
    /// </summary>
    /// <param name="JwtId">The JSON web token ID</param>
    void FlagAsUsedToken(string JwtId);

    /// <summary>
    /// Updated the revoked flag of the refresh token
    /// </summary>
    /// <param name="JwtId">The JSON web token ID</param>
    void FlagAsRevokedToken(string JwtId);
}
