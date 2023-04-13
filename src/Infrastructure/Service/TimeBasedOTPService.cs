using Application.Service;
using OtpNet;
using QRCoder;

namespace Infrastructure.Service;

internal class TimeBasedOTPService : ITimeBasedOTPService
{
    public byte[] GenerateQrCode(string email, byte[] secretKey, string applicationName)
    {
        // Generate a QR code image containing the secret key and user's email address.
        var qrCodeData = new QRCodeGenerator().CreateQrCode(
            $"otpauth://totp/{email}?secret={Base32Encoding.ToString(secretKey)}&issuer={applicationName}",
            QRCodeGenerator.ECCLevel.Q);

        var qrCode = new BitmapByteQRCode(qrCodeData);

        return qrCode.GetGraphic(20);
    }

    public bool VerifyTOTP(byte[] secretKey, string token)
    {
        return new Totp(secretKey).VerifyTotp(token, out _);
    }
}
