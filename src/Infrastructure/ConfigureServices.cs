﻿using Application;
using Infrastructure.Extension;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        ILoggingBuilder loggingBuilder,
        IWebHostEnvironment environment)
    {
        services.AddApplicationServices()
            .RegisterServiceImplementation(configuration, environment)
            .RegisterDatabaseContext(configuration, environment)
            .RegisterIdentity()
            .RegisterAuthentication(configuration, environment)
            .RegisterCors(configuration, environment)
            .RegisterHttpClient(configuration, environment)
            .RegisterOpenTelemetry(configuration, loggingBuilder, environment);

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
