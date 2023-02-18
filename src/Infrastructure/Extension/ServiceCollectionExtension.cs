﻿using Application.Common.Interface;
using gitViwe.ProblemDetail;
using gitViwe.ProblemDetail.Base;
using Infrastructure.Identity;
using Infrastructure.Persistance;
using Infrastructure.Persistance.Entity;
using Infrastructure.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Shared.Constant;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Extension;

internal static class ServiceCollectionExtension
{
    internal static IServiceCollection RegisterServiceImplementation(this IServiceCollection services)
    {
        services.AddScoped<ISuperHeroService, SuperHeroService>();
        services.AddTransient<IJWTTokenService, JWTTokenService>();
        services.AddTransient<IHubIdentityService, HubIdentityService>();

        return services;
    }

    internal static IServiceCollection RegisterDatabaseContext(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        if (environment.IsEnvironment("Docker"))
        {
            return services.AddDbContext<HubDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString(HubConfigurations.ConnectionString.PostgreSQL)!);
            });
        }

        return services.AddDbContext<HubDbContext>(options =>
        {
            // using an SQlite provider
            options.UseSqlite(configuration.GetConnectionString(HubConfigurations.ConnectionString.SQLite)!,
                                b => b.MigrationsAssembly("Infrastructure"));
        });
    }

    internal static IServiceCollection RegisterIdentity(this IServiceCollection services)
    {
        services.AddIdentity<HubIdentityUser, HubIdentityRole>(options =>
        {
            // require account to sign in
            options.SignIn.RequireConfirmedAccount = true;
            // password requirements
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
        }).AddEntityFrameworkStores<HubDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }

    internal static IServiceCollection RegisterAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // get the JWT key from the APP settings file
        var key = Encoding.ASCII.GetBytes(configuration[HubConfigurations.API.Secret]!);

        // create the parameters used to validate
        var tokenValidationParams = new TokenValidationParameters
        {
            ValidIssuer = configuration[HubConfigurations.API.ServerUrl],
            ValidAudiences = new string[]
            {
                configuration[HubConfigurations.API.ServerUrl]!,
                configuration[HubConfigurations.API.ClientUrl]!
            },
            // specify the security key used for 
            IssuerSigningKey = new SymmetricSecurityKey(key),
            // validates the signature of the key
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ValidateIssuer = true,
        };

        // add Token Validation Parameters as singleton service
        services.AddSingleton(tokenValidationParams);

        // create the parameters used to validate refreshing tokens
        var refreshTokenValidationParams = new TokenValidationParameters
        {
            ValidIssuer = configuration[HubConfigurations.API.ServerUrl],
            ValidAudiences = new string[]
            {
                configuration[HubConfigurations.API.ServerUrl]!,
                configuration[HubConfigurations.API.ClientUrl]!
            },
            // specify the security key used for 
            IssuerSigningKey = new SymmetricSecurityKey(key),
            // validates the signature of the key
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            // do not validate token expiry
            ValidateLifetime = false,
        };
        services.AddSingleton(new RefreshTokenValidationParameters(refreshTokenValidationParams));

        // configures authentication using JWT
        services.AddAuthentication(options =>
        {
            // specify default schema
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            // store bearer token on successful authentication
            options.SaveToken = true;
            // set the parameters used to validate
            options.TokenValidationParameters = tokenValidationParams;
            // set JWT authorization events
            options.Events = new JwtBearerEvents()
            {
                OnAuthenticationFailed = context =>
                {
                    DefaultProblemDetails response;

                    // JWT token has expired
                    response = context.Exception is SecurityTokenExpiredException
                        ? ProblemDetailFactory.CreateProblemDetails(context.HttpContext, StatusCodes.Status401Unauthorized, ErrorDescription.Authorization.ExpiredToken)
                        : ProblemDetailFactory.CreateProblemDetails(context.HttpContext, StatusCodes.Status401Unauthorized);

                    return context.Response.WriteAsync(JsonSerializer.Serialize(response));
                },
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    if (!context.Response.HasStarted)
                    {
                        var response = ProblemDetailFactory.CreateProblemDetails(context.HttpContext, StatusCodes.Status401Unauthorized, ErrorDescription.Authorization.Unauthorized);
                        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    }

                    return Task.CompletedTask;
                },
                OnForbidden = context =>
                {
                    var response = ProblemDetailFactory.CreateProblemDetails(context.HttpContext, StatusCodes.Status403Forbidden, ErrorDescription.Authorization.Unauthorized);
                    return context.Response.WriteAsync(JsonSerializer.Serialize(response));
                },
            };
        });

        // add authorization to apply policy
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

            // get all permissions from static properties
            foreach (var prop in typeof(HubPermissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                // get property value
                var propertyValue = prop.GetValue(null);

                if (propertyValue is not null)
                {
                    // add new permission policy
                    options.AddPolicy(propertyValue.ToString()!, policy => policy.RequireClaim(HubClaimTypes.Permission, propertyValue.ToString()!)
                           // add JWT Bearer authentication scheme to this policy
                           .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));
                }
            }
        });

        return services;
    }

    internal static IServiceCollection RegisterCors(this IServiceCollection services, IConfiguration configuration)
    {
        // add CORS policy https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-6.0
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        // allow requests from this URL
                        .WithOrigins(new string[]
                        {
                            configuration[HubConfigurations.API.ServerUrl]!.TrimEnd('/'),
                            configuration[HubConfigurations.API.ClientUrl]!.TrimEnd('/')
                        });
                });
        });

        return services;
    }
}
