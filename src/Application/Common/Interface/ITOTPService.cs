namespace Application.Common.Interface;

/// <summary>
/// Helper service for time-based one-time password (TOTP)
/// </summary>
public interface ITOTPService
{
    /// <summary>
    /// Creates a QR code image.
    /// </summary>
    /// <param name="email">The current user's email address.</param>
    /// <param name="secretKey">The key must be unique every time.</param>
    /// <param name="email">The current application name.</param>
    /// <returns>The QR code image as a <see cref="Byte"/> array.</returns>
    byte[] GenerateQrCode(string email, byte[] secretKey, string applicationName);

    /// <summary>
    /// Verifies the time-based one-time password (TOTP).
    /// </summary>
    /// <param name="secretKey">The secret key from the user.</param>
    /// <param name="token">The time-based one-time password.</param>
    /// <returns>True if the TOTP is valid.</returns>
    bool VerifyTOTP(byte[] secretKey, string token);
}
