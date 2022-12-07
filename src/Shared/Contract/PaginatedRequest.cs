namespace Shared.Contract;

/// <summary>
/// A unified paginated request type for the API endpoint
/// </summary>
public class PaginatedRequest
{
    /// <summary>
    /// The current page number
    /// </summary>
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    /// The number of items in a single page
    /// </summary>
    public int PageSize { get; set; } = 5;
}
