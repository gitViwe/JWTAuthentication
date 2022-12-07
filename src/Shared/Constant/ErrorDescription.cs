namespace Shared.Constant;

/// <summary>
/// Provides the common error messages
/// </summary>
public static class ErrorDescription
{
    /// <summary>
    /// Common error messages
    /// </summary>
    public static class Generic
    {
        public const string Validation = "One or more validation failures have occurred.";
        public const string Identity = "One or more security failures have occurred.";
    }

    /// <summary>
    /// Common authorization error messages
    /// </summary>
    public static class Authorization
    {
        public const string ExpiredToken = "The Token has expired.";
        public const string Unauthorized = "You are not Authorized.";
        public const string InternalServerError = "An unhandled error has occurred.";
        public const string Forbidden = "You are not authorized to access this resource.";
    }
}
