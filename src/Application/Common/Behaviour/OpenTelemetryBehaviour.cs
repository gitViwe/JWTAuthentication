using Shared.Constant;
using System.Diagnostics;

namespace Application.Common.Behaviour;

internal class OpenTelemetryBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var activity = Activity.Current ?? default;
        activity?.AddEvent(new ActivityEvent("Processing MediatR Request."));
        activity?.SetTag(HubOpenTelemetry.TagKey.MediatR.REQUEST_TYPE, request.GetType().Name);
        activity?.SetTag(HubOpenTelemetry.TagKey.MediatR.REQUEST_VALUE, HubOpenTelemetry.ObfuscateSensitiveData(request));

        return next();
    }
}
