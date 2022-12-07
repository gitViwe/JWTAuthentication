using Application.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationMediator();
        services.AddAppplicationSettingsSection(configuration);

        return services;
    }
}
