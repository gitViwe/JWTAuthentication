namespace Shared.Route;

/// <summary>
/// Provides the URLs for the API Endpoints
/// </summary>
public static class API
{
    /// <summary>
    /// The endpoint routes for "SuperHeroEndpoint"
    /// </summary>
    public static class SuperHeroEndpoint
    {
        public const string TAG_NAME = "Superhero";
        public const string GetPaginated = "api/superhero";
    }

    /// <summary>
    /// The endpoint routes for "AccountEndpoint"
    /// </summary>
    public static class AcccountEndpoint
    {
        public const string TAG_NAME = "Account";
        public const string Register = "api/account/register";
        public const string Login = "api/account/login";
        public const string Logout = "api/account/logout";
        public const string RefreshToken = "api/account/refresh-token";
        public const string QrCode = "api/account/qrcode";
        public const string TOTPVerify = "api/account/totp/verify";
        public const string Detail = "api/account/detail";
        public const string Picture = "api/account/picture";
    }
}
