using Shared.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Shared.Extension;

public static class ClaimsPrincipalExtension
{
    /// <summary>
    /// Gets the Email value from the claims
    /// </summary>
    public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Email)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Gets the Name value from the claims
    /// </summary>
    public static string GetFirstName(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirst(JwtRegisteredClaimNames.GivenName)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Gets the Surname value from the claims
    /// </summary>
    public static string GetLastName(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirst(JwtRegisteredClaimNames.FamilyName)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Gets the MobilePhone value from the claims
    /// </summary>
    public static string GetPhoneNumber(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirst(ClaimTypes.MobilePhone)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Gets the NameIdentifier value from the claims
    /// </summary>
    public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Gets the Sub value from the claims
    /// </summary>
    public static string GetUsername(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Checks if the JWT claims have expired
    /// </summary>
    public static bool HasExpiredClaims(this ClaimsPrincipal claimsPrincipal)
    {
        // verify that the expiry date has not passed
        if (long.TryParse(claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Exp)?.Value, out long expiry))
        {
            var expiryDate = Conversion.UnixTimeStampToDateTime(expiry);
            if (expiryDate > DateTime.UtcNow)
            {
                // token has not yet expired
                return false;
            }
        }
        return true;
    }
}
