using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Shared.Constant;
using System.Reflection;

namespace API.Extension;

internal static class ServiceCollectionExtension
{
    internal static IServiceCollection AddHubAPISwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            // add swagger documentation
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Hub API",
                Description = "A .NET 7 Web API demo project to showcase Minimal API, Clean Architecture, JSON Web Token authentication and Swagger / Open API documentation.",
                Version = "v1.0",
                Contact = new OpenApiContact()
                {
                    Name = "Viwe Nkepu",
                    Email = "viwe.nkepu@hotmail.com",
                    Url = new Uri(configuration[HubConfigurations.API.ClientUrl]!.TrimEnd('/'))
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // create authorization scheme to swagger UI
            var securitySchema = new OpenApiSecurityScheme
            {
                Description = "Authorize using JWT Bearer token.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            };

            // add authorization scheme to swagger UI
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securitySchema);

            // create security requirements
            var securityRequirement = new OpenApiSecurityRequirement
            {
                { securitySchema, new[] { JwtBearerDefaults.AuthenticationScheme } }
            };

            // add security requirements to swagger UI
            options.AddSecurityRequirement(securityRequirement);

            // get the file path for XML documentation
            var fileName = Assembly.GetEntryAssembly()!.GetName().Name + ".xml";
            var xmlCommentsFilePath = System.IO.Path.Combine(AppContext.BaseDirectory, fileName);
            // add XML documentation to swagger UI
            options.IncludeXmlComments(xmlCommentsFilePath, true);
        });

        return services;
    }
}
