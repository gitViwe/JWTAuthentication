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
        public const string GetPaginated = "api/superhero/{currentPage:required}&{pageSize:required}";
        public static string GetPaginatedEndpoint(int currentPage, int pageSize) => $"api/superhero/{currentPage}&{pageSize}";
    }

    /// <summary>
    /// The endpoint routes for "AccountEndpoint"
    /// </summary>
    public static class AcccountEndpoint
    {
        public const string PREFIX = "api/account/";
        public const string TAG_NAME = "Account";
        public const string Register = "register";
        public const string Login = "login";
        public const string RefreshToken = "refresh-token";
    }
}
