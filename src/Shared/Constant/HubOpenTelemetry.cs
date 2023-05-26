using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Shared.Constant;

public static class HubOpenTelemetry
{
    /// <summary>
    /// Converts the provided value into a <see cref="string"/> where the property values for: Email, Password and PasswordConfirmation are hidden.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to serialize.</typeparam>
    /// <param name="request">The value to convert.</param>
    /// <returns>A <see cref="string"/> representation of the value.</returns>
    public static string ObfuscateSensitiveData<TValue>(TValue request)
    {
        string text = JsonSerializer.Serialize(request);

        string pattern = @"(""(Email|Password|PasswordConfirmation|Token)"":\s*)""[^""]*""";
        string replacement = "$1\"*****\"";

        return Regex.Replace(text, pattern, replacement);
    }

    public static class Source
    {
        public const string MEDIATR = "MediatR";
        public const string AUTHAPI = "AuthenticationAPI";
        public const string MONGODB = "MongoDB.Driver.Core.Extensions.DiagnosticSources";
    }

    public static class MediatRActivitySource
    {
        private static readonly ActivitySource ActivitySource = new("MediatR");

        public static void StartActivity(string activityName, string eventName, Dictionary<string, object?> tags)
        {
            using var activity = ActivitySource.StartActivity(activityName);
            activity?.AddEvent(new ActivityEvent(eventName, tags: new ActivityTagsCollection(tags)));
        }
    }

    public static class AuthAPIActivitySource
    {
        private static readonly ActivitySource ActivitySource = new("AuthenticationAPI");

        public static void StartActivity(string activityName, string eventName)
        {
            using var activity = ActivitySource.StartActivity(activityName);
            activity?.AddEvent(new ActivityEvent(eventName));
        }

        public static void StartActivity(string activityName, string eventName, Exception exception)
        {
            Dictionary<string, object?> tagDictionary = new()
            {
                { "exception.message", exception.Message },
                { "exception.stacktrace", exception.StackTrace },
                { "exception.type", exception.GetType().FullName },
            };

            StartActivity(activityName, eventName, tagDictionary);
        }

        public static void StartActivity(string activityName, string eventName, Dictionary<string, object?> tags)
        {
            using var activity = ActivitySource.StartActivity(activityName);
            activity?.AddEvent(new ActivityEvent(eventName, tags: new ActivityTagsCollection(tags)));
        }
    }

    public static class TagKey
    {
        public static class MediatR
        {
            public const string REQUEST_TYPE = "auth_api.mediatr.request.type";
            public const string REQUEST_VALUE = "auth_api.mediatr.request.value";
            public const string RESPONSE_STATUS_CODE = "auth_api.mediatr.response.status_code";
            public const string RESPONSE_MESSAGE = "auth_api.mediatr.response.message";
        }

        public static class HubUser
        {
            public const string USER_ID = "user_id";
            public const string JWT_ID = "jwt_id";
            public const string JWT_ISSUER = "jwt_issuer";
            public const string JWT_AUDIENCE = "jwt_audience";
        }
    }

    public static class AspNetCoreInstrumentation
    {
        public readonly static string[] FilterUrls =
        {
            "/swagger/v1/swagger.json",
            "/_vs/browserLink",
            "/_framework/aspnetcore-browser-refresh.js",
            "/swagger/index.html",
            "/swagger/favicon-32x32.png",
            "/favicon.ico",
            "/swagger/swagger-ui-bundle.js",
            "/swagger/swagger-ui-bundle.js",
            "/swagger/swagger-ui.css",
            "/"
        };
    }
}
