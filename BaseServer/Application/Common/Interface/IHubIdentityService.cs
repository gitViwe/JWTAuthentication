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
    /// <returns>A <see cref="Response{TokenResponse}"/> where the data type is <see cref="TokenResponse"/></returns>
    Task<IResponse> RegisterAsync(RegisterRequest request, CancellationToken token);

    /// <summary>
    /// Request a new token if the current token is invalid or expired
    /// </summary>
    /// <param name="request">This is the required user information to request a new token</param>
    /// <param name="token">Propagates notification that operations should be canceled</param>
    /// <returns>A <see cref="Response{TokenResponse}"/> where the data type is <see cref="TokenResponse"/></returns>
    /// <exception cref="Shared.Exception.HubIdentityException"></exception>
    Task<IResponse> RefreshToken(TokenRequest request, CancellationToken token);

    /// <summary>
    /// Login an existing system user
    /// </summary>
    /// <param name="request">This is the required user information to login</param>
    /// /// <param name="token">Propagates notification that operations should be canceled</param>
    /// <returns>A <see cref="Response{TokenResponse}"/> where the data type is <see cref="TokenResponse"/></returns>
    Task<IResponse> LoginUserAsync(LoginRequest request, CancellationToken token);
}
