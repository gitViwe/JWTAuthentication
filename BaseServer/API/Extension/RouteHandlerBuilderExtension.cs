using Shared.Wrapper;
using System.Net.Mime;
using System.Net;

namespace API.Extension;

internal static class RouteHandlerBuilderExtension
{
    /// <summary>
    /// Adds response description from the global exception handler
    /// </summary>
    public static RouteHandlerBuilder ProducesDefaultHubResponses(this RouteHandlerBuilder builder)
    {
        builder.Produces<IResponse>((int)HttpStatusCode.BadRequest, MediaTypeNames.Application.Json);
        builder.Produces<IResponse>((int)HttpStatusCode.Forbidden, MediaTypeNames.Application.Json);

        return builder;

    }
}
