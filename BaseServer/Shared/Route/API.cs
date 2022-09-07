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
        public const string GetAll = "api/superhero/all/{currentPage:required}&{pageSize:required}";
        public static string GetAllPaginated(int currentPage, int pageSize) => $"api/superhero/all/{currentPage}&{pageSize}";
    }
}
