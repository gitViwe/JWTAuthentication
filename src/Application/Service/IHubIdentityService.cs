using Shared.Contract.Identity;

namespace Application.Service;

/// <summary>
/// Facilitates the authentication of system users
/// </summary>
public interface IHubIdentityService
{
    /// <summary>
    /// Register a new user on the system
    /// </summary>
    /// <param name="request">This is the required user information to register</param>
    /// <param name="token">Propagates notification that operations should be cancelled</param>
    /// <returns>The <see cref="TokenResponse"/></returns>
    /// <exception cref="UnauthorizedException"></exception>
    Task<IResponse<TokenResponse>> RegisterAsync(RegisterRequest request, CancellationToken token);

    /// <summary>
    /// Request a new token if the current token is invalid or expired
    /// </summary>
    /// <param name="request">This is the required user information to request a new token</param>
    /// <param name="token">Propagates notification that operations should be cancelled</param>
    /// <returns>The <see cref="TokenResponse"/></returns>
    /// <exception cref="UnauthorizedException"></exception>
    Task<IResponse<TokenResponse>> RefreshToken(TokenRequest request, CancellationToken token);

    /// <summary>
    /// Login an existing system user
    /// </summary>
    /// <param name="request">This is the required user information to login</param>
    /// /// <param name="token">Propagates notification that operations should be cancelled</param>
    /// <returns>The <see cref="TokenResponse"/></returns>
    /// <exception cref="UnauthorizedException"></exception>
    Task<IResponse<TokenResponse>> LoginUserAsync(LoginRequest request, CancellationToken token);

    /// <summary>
    /// Logout the current system user
    /// </summary>
    /// <param name="tokenId">The current user's JWT token id</param>
    /// <param name="token">Propagates notification that operations should be cancelled</param>
    /// <exception cref="UnauthorizedException"></exception>
    Task LogoutUserAsync(string tokenId, CancellationToken token);

    /// <summary>
    /// Get a QrCode image for the current user
    /// </summary>
    /// <param name="request">The current user's JWT and refresh token</param>
    /// <param name="token">Propagates notification that operations should be cancelled</param>
    /// <returns>The image as an <see cref="QrCodeImageResponse"/> instance</returns>
    /// <exception cref="UnauthorizedException"></exception>
    Task<IResponse<QrCodeImageResponse>> GetQrCodeImageAsync(QrCodeImageRequest request, CancellationToken token);

    /// <summary>
    /// Verifies the time-based one-time password (TOTP)
    /// </summary>
    /// <param name="email">The user's email</param>
    /// <param name="totp">The user's TOTP</param>
    /// <param name="token">Propagates notification that operations should be cancelled</param>
    /// <returns>The response message</returns>
    /// <exception cref="UnauthorizedException"></exception>
    Task<IResponse> ValidateTOTPAsync(string email, string totp, CancellationToken token);

    /// <summary>
    /// Updates the user's first name and last name
    /// </summary>
    /// <param name="email">The user's email</param>
    /// <param name="request">The current user's details</param>
    /// <param name="token">Propagates notification that operations should be cancelled</param>
    /// <returns>The <see cref="TokenResponse"/></returns>
    /// <exception cref="UnauthorizedException"></exception>
    Task<IResponse<TokenResponse>> UpdateUserAsync(string email, UpdateUserRequest request, CancellationToken token);

    /// <summary>
    /// Updates the user's profile image
    /// </summary>
    /// <param name="userId">The user's user id</param>
    /// <param name="request">The current user's details</param>
    /// <param name="token">Propagates notification that operations should be cancelled</param>
    /// <returns>The <see cref="TokenResponse"/></returns>
    /// <exception cref="UnauthorizedException"></exception>
    Task<IResponse<TokenResponse>> UploadImageAsync(string userId, UploadImageRequest request, CancellationToken token);

    /// <summary>
    /// Get the current user's details
    /// </summary>
    /// <param name="userId">The user's user id</param>
    /// <param name="token">Propagates notification that operations should be cancelled</param>
    /// <returns>The <see cref="UserDetailResponse"/></returns>
    /// <exception cref="UnauthorizedException"></exception>
    Task<IResponse<UserDetailResponse>> GetUserDetailAsync(string userId, CancellationToken token);
}
