using Application;
using Infrastructure.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationServices(configuration);
        services.RegisterServiceImplementation();
        services.RegisterDatabaseContext(configuration);
        services.RegisterIdentity();
        services.RegisterAuthentication(configuration);

        return services;
    }
}