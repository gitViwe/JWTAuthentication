using Application;
using Infrastructure.Extension;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddApplicationServices()
            .RegisterServiceImplementation(configuration, environment)
            .RegisterDatabaseContext(configuration, environment)
            .RegisterIdentity()
            .RegisterAuthentication(configuration, environment)
            .RegisterCors(configuration, environment)
            .RegisterHttpClient(configuration)
            .RegisterOpenTelemetry(configuration, environment);

        return services;
    }

    public static Task UseInfrastructureServicesAsync(this IHost host, IHostEnvironment environment)
    {
        if (environment.IsDevelopment() || environment.IsEnvironment("Docker"))
        {
            return host.ApplyMigrationAsync();
        }
        return Task.CompletedTask;
    }
}
