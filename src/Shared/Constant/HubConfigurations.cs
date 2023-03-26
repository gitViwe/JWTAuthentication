namespace Shared.Constant;

/// <summary>
/// Provides the keys to get the values from AppSettings
/// </summary>
public static class HubConfigurations
{
    /// <summary>
    /// Provides the keys to get the values from the APIConfiguration section
    /// </summary>
    public static class API
    {
        public const string ApplicationName = "APIConfiguration:ApplicationName";
        public const string Secret = "APIConfiguration:Secret";
        public const string ClientUrl = "APIConfiguration:ClientUrl";
        public const string ServerUrl = "APIConfiguration:ServerUrl";
        public const string TokenExpityInMinutes = "APIConfiguration:TokenExpityInMinutes";
        public const string RefreshTokenExpityInMinutes = "APIConfiguration:RefreshTokenExpityInMinutes";
    }

    /// <summary>
    /// Provides the keys to get the values from the ConnectionStrings section
    /// </summary>
    public static class ConnectionString
    {
        public const string SQL = "SQL";
        public const string SQLite = "SQLite";
        public const string CosmosDb = "CosmosDb";
        public const string PostgreSQL = "PostgreSQL";
    }
}
