using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Extension;

internal static class HostExtension
{
    internal static async Task ApplyMigrationAsync(this IHost host)
    {
        using IServiceScope scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<HubDbContext>();
        await context.Database.MigrateAsync();
    }
}