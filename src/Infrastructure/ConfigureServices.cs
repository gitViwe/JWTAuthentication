using Application;
using Infrastructure.Extension;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddApplicationServices(configuration)
            .RegisterServiceImplementation()
            .RegisterDatabaseContext(configuration, environment)
            .RegisterIdentity()
            .RegisterAuthentication(configuration, environment)
            .RegisterCors(configuration, environment)
            .RegisterHttpClient(configuration);

        return services;
    }

    public static Task UseInfrastructureServicesAsync(this IHost host, IHostEnvironment environment)
    {
        if (environment.IsEnvironment("Docker"))
        {
            return host.CreateDatabaseAsync();
        }
        else
        {
            return host.ApplyMigrationAsync();
        }
    }
}
