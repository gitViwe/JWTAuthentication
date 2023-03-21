using Application.Common.Interface;
using gitViwe.Shared;
using Microsoft.AspNetCore.Hosting;
using Shared.Contract;
using Shared.Contract.SuperHero;
using System.Text.Json;

namespace Infrastructure.Service;

internal class SuperHeroService : ISuperHeroService
{
    private const string HERO_JSON_FILE = "sample-data/superheroes.json";
    private readonly IWebHostEnvironment _environment;

    public SuperHeroService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<IEnumerable<SuperHeroResponse>> GetEnumerableAsync(CancellationToken token)
    {
        var filePath = Path.Combine(_environment.WebRootPath, HERO_JSON_FILE);
        var jsonString = await File.ReadAllTextAsync(filePath, token);
        var output = JsonSerializer.Deserialize<IEnumerable<SuperHeroResponse>>(jsonString);
        return output ?? Enumerable.Empty<SuperHeroResponse>();
    }

    public async Task<PaginatedResponse<SuperHeroResponse>> GetPaginatedAsync(PaginatedRequest request, CancellationToken token)
    {
        var output = await GetEnumerableAsync(token);

        return output.Any()
            ? PaginatedResponse<SuperHeroResponse>.Success(output, request.CurrentPage, request.PageSize)
            : PaginatedResponse<SuperHeroResponse>.Fail();
    }
}
