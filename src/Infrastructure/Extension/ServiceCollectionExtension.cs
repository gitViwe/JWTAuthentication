using Application.ApiClient;
using Application.Common.DevelopmentMock;
using Application.Service;
using gitViwe.ProblemDetail;
using gitViwe.ProblemDetail.Base;
using Infrastructure.ApiClient;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Entity;
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
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Shared.Constant;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Extension;

internal static class ServiceCollectionExtension
{
    internal static IServiceCollection RegisterServiceImplementation(this IServiceCollection services)
    {
        services.AddScoped(typeof(MongoDBRepository<>));
        services.AddScoped<ISuperHeroService, SuperHeroService>();
        services.AddTransient<ITimeBasedOTPService, TimeBasedOTPService>();
        services.AddTransient<IJsonWebTokenService, JsonWebTokenService>();
        services.AddTransient<IHubIdentityService, HubIdentityService>();

        return services;
    }

    internal static IServiceCollection RegisterDatabaseContext(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        return services.AddDbContext<HubDbContext>(options =>
        {
            if (environment.IsEnvironment("Docker"))
            {
                // using an PostgreSQL provider
                options.UseNpgsql(configuration.GetConnectionString(HubConfigurations.ConnectionString.PostgreSQL)!);
            }
            else if (environment.IsDevelopment())
            {
                // using an SQlite provider
                options.UseSqlite(configuration.GetConnectionString(HubConfigurations.ConnectionString.SQLite)!,
                                    b => b.MigrationsAssembly("Infrastructure"));
            }
            else
            {
                // using an CosmosDb provider
                options.UseCosmos(configuration.GetConnectionString(HubConfigurations.ConnectionString.CosmosDb)!, "SocialHub");
            }
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

    internal static IServiceCollection RegisterAuthentication(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
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
            ValidateAudience = environment.IsProduction(),
            ValidateIssuer = environment.IsProduction(),
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
            ValidateAudience = environment.IsProduction(),
            ValidateIssuer = environment.IsProduction(),
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
                    HubOpenTelemetry.AuthAPIActivitySource.StartActivity("JwtBearerEvents", "OnAuthenticationFailed");
                    DefaultProblemDetails response;

                    // JWT token has expired
                    response = context.Exception is SecurityTokenExpiredException
                        ? ProblemDetailFactory.CreateProblemDetails(context.HttpContext, StatusCodes.Status401Unauthorized, ErrorDescription.Authorization.ExpiredToken)
                        : ProblemDetailFactory.CreateProblemDetails(context.HttpContext, StatusCodes.Status401Unauthorized);

                    return context.Response.WriteAsync(JsonSerializer.Serialize(response));
                },
                OnChallenge = context =>
                {
                    HubOpenTelemetry.AuthAPIActivitySource.StartActivity("JwtBearerEvents", "OnChallenge");
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
                    HubOpenTelemetry.AuthAPIActivitySource.StartActivity("JwtBearerEvents", "OnForbidden");
                    var response = ProblemDetailFactory.CreateProblemDetails(context.HttpContext, StatusCodes.Status403Forbidden, ErrorDescription.Authorization.Unauthorized);
                    return context.Response.WriteAsync(JsonSerializer.Serialize(response));
                },
                OnTokenValidated = context =>
                {
                    string userId = context?.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                    string username = context?.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? string.Empty;
                    string email = context?.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value ?? string.Empty;

                    Dictionary<string, object?> authTagDictionary = new()
                    {
                        { HubOpenTelemetry.TagKey.HubUser.USER_ID, userId },
                        { HubOpenTelemetry.TagKey.HubUser.USER_NAME, username },
                        { HubOpenTelemetry.TagKey.HubUser.USER_EMAIL, email },
                    };

                    HubOpenTelemetry.AuthAPIActivitySource.StartActivity("JwtBearerEvents", "OnTokenValidated", authTagDictionary);

                    return Task.CompletedTask;
                }
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

    internal static IServiceCollection RegisterCors(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        // add CORS policy https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-6.0
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    if (environment.IsDevelopment() || environment.IsEnvironment("Docker"))
                    {
                        builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                    }
                    else
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
                    }
                });
        });

        return services;
    }

    internal static IServiceCollection RegisterHttpClient(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        if (!environment.IsProduction())
        {
            return services.AddScoped<IImageHostingClient>(_ => new LocalImageHostingClient(Path.Combine(environment.WebRootPath, "image", "user-image")));
        }

        services.AddHttpClient<IImageHostingClient, ImgBBClient>(client =>
        {
            client.BaseAddress = new Uri(configuration[HubConfigurations.APIClient.ImgBB.BaseUrl]!);
        });

        return services;
    }

    public static void RegisterOpenTelemetry(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var resource = ResourceBuilder.CreateDefault().AddService(configuration[HubConfigurations.API.ApplicationName]!);

        services.AddOpenTelemetry().WithTracing(builder =>
        {
            builder.AddSource(HubOpenTelemetry.Source.MEDIATR, HubOpenTelemetry.Source.AUTHAPI, HubOpenTelemetry.Source.MONGODB)
                   .SetResourceBuilder(resource)
                   .AddHttpClientInstrumentation(options =>
                   {
                       options.RecordException = true;
                       options.EnrichWithException = (activity, exception) => activity?.RecordException(exception);
                       options.EnrichWithHttpRequestMessage = (activity, request) =>
                       {
                           string absPath = request.RequestUri?.AbsolutePath ?? string.Empty;
                           if (absPath.Contains("upload"))
                           {
                               activity?.AddEvent(new ActivityEvent("Processing Image Upload Request."));
                           }
                       };
                   })
                   .AddEntityFrameworkCoreInstrumentation(x => x.SetDbStatementForText = true)
                   .AddAspNetCoreInstrumentation(options =>
                   {
                       options.RecordException = true;
                       options.EnrichWithException = (activity, exception) => activity.RecordException(exception);
                       options.Filter = (context) => !HubOpenTelemetry.AspNetCoreInstrumentation.FilterUrls.Contains(context.Request.Path.Value);

                   });

            if (environment.IsProduction())
            {
                builder.AddOtlpExporter(option =>
                {
                    option.Endpoint = new Uri(configuration[HubConfigurations.OpenTelemetry.Honeycomb.Endpoint]!);
                    option.Headers = configuration[HubConfigurations.OpenTelemetry.Honeycomb.Headers]!;
                });
            }
            else
            {
                builder.AddJaegerExporter(options =>
                {
                    options.AgentHost = configuration[HubConfigurations.OpenTelemetry.Jaeger.AgentHost]!;
                    options.AgentPort = int.Parse(configuration[HubConfigurations.OpenTelemetry.Jaeger.AgentPort]!);
                });
            }
        });
    }
}
