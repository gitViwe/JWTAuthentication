using Shared.Contract.SuperHero;

namespace Application.Common.Interface;

/// <summary>
/// Facilitates the sample data for heroes
/// </summary>
public interface ISuperHeroService
{
    /// <summary>
    /// Request a paginated list of heroes
    /// </summary>
    /// <param name="token">Propagates notification that operations should be canceled</param>
    /// <returns>A <see cref="PaginatedResponse{SuperHeroResponse}"/> where the data type is a collection of <see cref="SuperHeroResponse"/></returns>
    Task<PaginatedResponse<SuperHeroResponse>> GetPaginatedAsync(PaginatedRequest request, CancellationToken token);

    /// <summary>
    /// Request the complete list of heroes
    /// </summary>
    /// <param name="token">Propagates notification that operations should be canceled</param>
    /// <returns>A <see cref="IEnumerable{SuperHero}"/> where the data type is a collection of <see cref="SuperHeroResponse"/></returns>
    Task<IEnumerable<SuperHeroResponse>> GetEnumerableAsync(CancellationToken token);
}
