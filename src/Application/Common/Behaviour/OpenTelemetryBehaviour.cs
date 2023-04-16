using Shared.Constant;

namespace Application.Common.Behaviour;

internal class OpenTelemetryBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Dictionary<string, object?> tagDictionary = new()
        {
            { HubOpenTelemetry.TagKey.MediatR.REQUEST_TYPE, request.GetType().Name },
            { HubOpenTelemetry.TagKey.MediatR.REQUEST_VALUE, HubOpenTelemetry.ObfuscateSensitiveData(request) },
        };

        HubOpenTelemetry.MediatRActivitySource.StartActivity("PipelineBehavior", "Starting MediatR Request.", tagDictionary);

        return next();
    }
}
