using Application.Common.Behaviour;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Extension;

internal static class ServiceCollectionExtension
{
    internal static IServiceCollection AddApplicationMediator(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.AddOpenBehavior(typeof(OpenTelemetryBehaviour<,>));
            config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
        });
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
