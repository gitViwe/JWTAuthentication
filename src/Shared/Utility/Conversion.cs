namespace Shared.Utility;

/// <summary>
/// Provides common data conversion methods
/// </summary>
public static class Conversion
{
    /// <summary>
    /// This converts the 64 byte hash into the string hex representation of byte values 
    /// (shown by default as 2 hex characters per byte) that looks like 
    /// "FB-2F-85-C8-85-67-F3-C8-CE-9B-79-9C-7C-54-64-2D-0C-7B-41-F6...", each pair represents
    /// the byte value of 0-255. Removing the "-" separator creates a 128 character string 
    /// representation in hexadecimal
    /// </summary>
    /// <param name="value">The byte array to convert</param>
    public static string ByteArrayToString(byte[] value)
    {
        return BitConverter.ToString(value).Replace("-", "");
    }

    /// <summary>
    /// This converts the hex string to a byte array
    /// </summary>
    /// <param name="hex">The hexadecimal string to convert</param>
    /// <returns>A <see cref="byte"/> array value representing the string data</returns>
    public static byte[] StringToByteArray(string hex)
    {
        int NumberChars = hex.Length;
        byte[] bytes = new byte[NumberChars / 2];
        for (int i = 0; i < NumberChars; i += 2)
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        return bytes;
    }

    /// <summary>
    /// Creates a random string
    /// </summary>
    /// <param name="length">The number of characters the string will have</param>
    /// <returns>A random string of the specified length</returns>
    public static string RandomString(int length)
    {
        var random = new Random();
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(x => x[random.Next(x.Length)]).ToArray());
    }

    /// <summary>
    /// Gets the <see cref="DateTime"/> value from the time stamp
    /// </summary>
    /// <param name="unixTimeStamp">The unix time stamp to convert</param>
    /// <returns>A <see cref="DateTime"/> value</returns>
    public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();

        return dateTimeVal;
    }

    /// <summary>
    /// This converts the base 64 string to a byte array
    /// </summary>
    /// <param name="payload">The base 64 string without padding</param>
    /// <returns>A <see cref="byte"/> array value representing the string data</returns>
    public static byte[] ParseBase64WithoutPadding(string payload)
    {
        payload = payload.Trim().Replace('-', '+').Replace('_', '/');
        var base64 = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
        return Convert.FromBase64String(base64);
    }
}
