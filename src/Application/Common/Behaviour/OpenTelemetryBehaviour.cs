using Shared.Constant;

namespace Application.Common.Behaviour;

internal class OpenTelemetryBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResponse
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Dictionary<string, object?> requestTagDictionary = new()
        {
            { HubOpenTelemetry.TagKey.MediatR.REQUEST_TYPE, request.GetType().Name },
            { HubOpenTelemetry.TagKey.MediatR.REQUEST_VALUE, Conversion.ToObfuscatedString(request, "Email", "Password", "PasswordConfirmation", "Token") },
        };

        HubOpenTelemetry.MediatRActivitySource.StartActivity("PipelineBehavior", "Starting MediatR Request.", requestTagDictionary);

        var response = await next();

        Dictionary<string, object?> responseTagDictionary = new()
        {
            { HubOpenTelemetry.TagKey.MediatR.RESPONSE_STATUS_CODE, response.StatusCode },
            { HubOpenTelemetry.TagKey.MediatR.RESPONSE_MESSAGE, response.Message },
        };

        HubOpenTelemetry.MediatRActivitySource.StartActivity("PipelineBehavior", "Completing MediatR Request.", responseTagDictionary);

        return response;
    }
}
