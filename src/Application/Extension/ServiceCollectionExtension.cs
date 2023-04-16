using Application.Common.Behaviour;
using Application.Configuration;
using Microsoft.Extensions.Configuration;
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

    internal static IServiceCollection AddAppplicationSettingsSection(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<APIConfiguration>(configuration.GetSection(nameof(APIConfiguration)));

        return services;
    }
}
