using gitViwe.ProblemDetail;
using gitViwe.Shared;
using Microsoft.AspNetCore.Diagnostics;
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

                int statusCode = StatusCodes.Status500InternalServerError;
                string response = JsonSerializer.Serialize(ProblemDetailFactory.CreateProblemDetails(context, statusCode));

                if (handlerFeature is not null)
                {
                    if (handlerFeature.Error is ForbiddenException forbidden)
                    {
                        statusCode = StatusCodes.Status403Forbidden;
                        response = JsonSerializer.Serialize(ProblemDetailFactory.CreateProblemDetails(context, statusCode, forbidden.Detail));
                        logger.LogWarning(forbidden, "A forbidden exception occurred. Problem detail: {response}", response);
                    }
                    else if (handlerFeature.Error is NotFoundException notFound)
                    {
                        statusCode = StatusCodes.Status404NotFound;
                        response = JsonSerializer.Serialize(ProblemDetailFactory.CreateProblemDetails(context, statusCode, notFound.Detail));
                        logger.LogWarning(notFound, "A not found exception occurred. Problem detail: {response}", response);
                    }
                    else if (handlerFeature.Error is UnauthorizedException unauthorized)
                    {
                        statusCode = StatusCodes.Status401Unauthorized;
                        response = JsonSerializer.Serialize(ProblemDetailFactory.CreateProblemDetails(context, statusCode, unauthorized.Detail));
                        logger.LogInformation(unauthorized, "An unauthorized exception occurred. Problem detail: {response}", response);
                    }
                    else if (handlerFeature.Error is ValidationException validation)
                    {
                        statusCode = StatusCodes.Status400BadRequest;
                        response = JsonSerializer.Serialize(ProblemDetailFactory.CreateValidationProblemDetails(context, statusCode, validation.ToDictionary()));
                        logger.LogInformation(validation, "A validation exception occurred. Problem detail: {response}", response);
                    }
                    else
                    {
                        logger.LogError(handlerFeature.Error, "A unhandled exception occurred. Problem detail: {response}", response);
                    }
                }
                else
                {
                    logger.LogError("A unhandled exception occurred. Problem detail: {response}", response);
                }

                await context.Response.WriteAsync(response);
                await context.Response.CompleteAsync();
            });
        });
    }
}
