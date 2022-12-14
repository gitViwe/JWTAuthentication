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
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        return services;
    }

    internal static IServiceCollection AddAppplicationSettingsSection(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<APIConfiguration>(configuration.GetSection(nameof(APIConfiguration)));

        return services;
    }
}
