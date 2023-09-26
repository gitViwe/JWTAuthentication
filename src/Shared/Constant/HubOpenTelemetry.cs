using System.Diagnostics;

namespace Shared.Constant;

public static class HubOpenTelemetry
{
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

            using var activity = ActivitySource.StartActivity(activityName);
            activity?.SetStatus(ActivityStatusCode.Error);
            activity?.AddEvent(new ActivityEvent(eventName, tags: new ActivityTagsCollection(tagDictionary)));
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
            public const string USER_ID = "auth_api.user_id";
            public const string JWT_ID = "auth_api.jwt_id";
            public const string JWT_ISSUER = "auth_api.jwt_issuer";
            public const string JWT_AUDIENCE = "auth_api.jwt_audience";
        }

        public static class API
        {
            public const string PROBLEM_DETAIL = "auth_api.problem_detail";
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
