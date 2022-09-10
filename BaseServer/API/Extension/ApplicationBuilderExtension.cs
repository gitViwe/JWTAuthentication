using Microsoft.AspNetCore.Diagnostics;
using Shared.Wrapper;
using System.Net.Mime;
using System.Net;
using System.Text.Json;
using Shared.Constant;
using Shared.Exception;

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
                int statusCode = (int)HttpStatusCode.InternalServerError;
                string contentType = MediaTypeNames.Application.Json;
                IResponse response = Response.Fail(ErrorDescription.Authorization.InternalServerError);

                // attempt to get exception details
                var handlerFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                if (handlerFeature is not null)
                {
                    // log error message
                    logger.LogError(handlerFeature.Error, handlerFeature.Error.Message);

                    if (handlerFeature.Error is HubValidationException validationException)
                    {
                        statusCode = (int)HttpStatusCode.BadRequest;
                        contentType = MediaTypeNames.Application.Json;
                        response = Response.Fail(validationException.ErrorMessages);
                    }
                    else if (handlerFeature.Error is HubIdentityException identityException)
                    {
                        statusCode = (int)HttpStatusCode.Forbidden;
                        contentType = MediaTypeNames.Application.Json;
                        response = Response.Fail(identityException.Message);
                    }
                }

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = contentType;
                var content = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(content);
                await context.Response.CompleteAsync();
            });
        });
    }
}
