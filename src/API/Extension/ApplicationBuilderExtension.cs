using gitViwe.ProblemDetail;
using gitViwe.Shared;
using Microsoft.AspNetCore.Diagnostics;
using Shared.Constant;
using System.Text.Json;

namespace API.Extension;

internal static class ApplicationBuilderExtension
{
    /// <summary>
    /// Global exception handler middleware
    /// </summary>
    internal static void UseHubExceptionHandler(this IApplicationBuilder app, ILogger logger)
    {
        app.UseExceptionHandler(options =>
        {
            options.Run(async context =>
            {
                var handlerFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                object response = ProblemDetailFactory.CreateProblemDetails(context, StatusCodes.Status500InternalServerError);

                if (handlerFeature is not null)
                {
                    HubOpenTelemetry.AuthAPIActivitySource.StartActivity("HubExceptionHandler", "ExceptionHandled", handlerFeature.Error);

                    if (handlerFeature.Error is ForbiddenException forbidden)
                    {
                        response = ProblemDetailFactory.CreateProblemDetails(context, StatusCodes.Status403Forbidden, forbidden.Detail);
                        logger.LogWarning(forbidden, "A forbidden exception occurred.\n {response}", response.ToString());
                    }
                    else if (handlerFeature.Error is NotFoundException notFound)
                    {
                        response = ProblemDetailFactory.CreateProblemDetails(context, StatusCodes.Status404NotFound, notFound.Detail);
                        logger.LogWarning(notFound, "A not found exception occurred.\n {response}", response.ToString());
                    }
                    else if (handlerFeature.Error is UnauthorizedException unauthorized)
                    {
                        response = ProblemDetailFactory.CreateProblemDetails(context, StatusCodes.Status401Unauthorized, unauthorized.Detail);
                        logger.LogInformation(unauthorized, "An unauthorized exception occurred.\n {response}", response.ToString());
                    }
                    else if (handlerFeature.Error is ValidationException validation)
                    {
                        response = ProblemDetailFactory.CreateValidationProblemDetails(context, StatusCodes.Status400BadRequest, validation.ToDictionary());
                        logger.LogInformation(validation, "A validation exception occurred.\n {response}", response.ToString());
                    }
                    else
                    {
                        logger.LogError(handlerFeature.Error, "A unhandled exception occurred.\n {response}", response.ToString());
                    }
                }
                else
                {
                    logger.LogError("A unhandled exception occurred.\n {response}", response.ToString());
                }

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                await context.Response.CompleteAsync();
            });
        });
    }
}
