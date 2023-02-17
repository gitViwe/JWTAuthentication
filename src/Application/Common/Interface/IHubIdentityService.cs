using Shared.Contract.Identity;

namespace Application.Common.Interface;

/// <summary>
/// Facilitates the authentication of system users
/// </summary>
public interface IHubIdentityService
{
    /// <summary>
    /// Register a new user on the system
    /// </summary>
    /// <param name="request">This is the required user information to register</param>
    /// <param name="token">Propagates notification that operations should be canceled</param>
    /// <returns>The <see cref="TokenResponse"/></returns>
    /// <exception cref="UnauthorizedException"></exception>
    Task<IResponse> RegisterAsync(IRegisterRequest request, CancellationToken token);

    /// <summary>
    /// Request a new token if the current token is invalid or expired
    /// </summary>
    /// <param name="request">This is the required user information to request a new token</param>
    /// <param name="token">Propagates notification that operations should be canceled</param>
    /// <returns>The <see cref="TokenResponse"/></returns>
    /// <exception cref="UnauthorizedException"></exception>
    Task<IResponse> RefreshToken(ITokenRequest request, CancellationToken token);

    /// <summary>
    /// Login an existing system user
    /// </summary>
    /// <param name="request">This is the required user information to login</param>
    /// /// <param name="token">Propagates notification that operations should be canceled</param>
    /// <returns>The <see cref="TokenResponse"/></returns>
    /// <exception cref="UnauthorizedException"></exception>
    Task<IResponse> LoginUserAsync(ILoginRequest request, CancellationToken token);

    /// <summary>
    /// Logout the current system user
    /// </summary>
    /// <param name="tokenId">The current user's JWT token id</param>
    /// <param name="token">Propagates notification that operations should be canceled</param>
    /// <exception cref="UnauthorizedException"></exception>
    Task LogoutUserAsync(string tokenId, CancellationToken token);
}
