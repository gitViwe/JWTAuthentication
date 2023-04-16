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

        string pattern = @"(""(Email|Password|PasswordConfirmation)"":\s*)""[^""]*""";
        string replacement = "$1\"*****\"";

        return Regex.Replace(text, pattern, replacement);
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

    public static class TagKey
    {
        public static class MediatR
        {
            public const string REQUEST_TYPE = "auth_api.mediatr.request.type";
            public const string REQUEST_VALUE = "auth_api.mediatr.request.value";
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
