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
                string response = JsonSerializer.Serialize(ProblemDetailFactory.CreateProblemDetails(context, StatusCodes.Status500InternalServerError));

                if (handlerFeature is not null)
                {
                    if (handlerFeature.Error is ForbiddenException forbidden)
                    {
                        response = JsonSerializer.Serialize(ProblemDetailFactory.CreateProblemDetails(context, StatusCodes.Status403Forbidden, forbidden.Detail));
                        logger.LogWarning(forbidden, "A forbidden exception occurred. Problem detail: {response}", response);
                    }
                    else if (handlerFeature.Error is NotFoundException notFound)
                    {
                        response = JsonSerializer.Serialize(ProblemDetailFactory.CreateProblemDetails(context, StatusCodes.Status404NotFound, notFound.Detail));
                        logger.LogWarning(notFound, "A not found exception occurred. Problem detail: {response}", response);
                    }
                    else if (handlerFeature.Error is UnauthorizedException unauthorized)
                    {
                        response = JsonSerializer.Serialize(ProblemDetailFactory.CreateProblemDetails(context, StatusCodes.Status401Unauthorized, unauthorized.Detail));
                        logger.LogInformation(unauthorized, "An unauthorized exception occurred. Problem detail: {response}", response);
                    }
                    else if (handlerFeature.Error is ValidationException validation)
                    {
                        response = JsonSerializer.Serialize(ProblemDetailFactory.CreateValidationProblemDetails(context, StatusCodes.Status400BadRequest, validation.ToDictionary()));
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
