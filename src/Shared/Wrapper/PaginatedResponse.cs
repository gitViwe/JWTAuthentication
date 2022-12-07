namespace Shared.Wrapper;

/// <summary>
/// A unified return type for the API endpoint
/// </summary>
/// <typeparam name="TData">The data type returned from the request</typeparam>
public class PaginatedResponse<TData> : Response where TData : class, new()
{
    /// <summary>
    /// Instantiate a new page-able result to return 
    /// </summary>
    /// <param name="succeeded">Flags whether the process was successful</param>
    internal PaginatedResponse(bool succeeded)
        :base(succeeded)
    {
        Data = Array.Empty<TData>();
        TotalCount = 0;
        CurrentPage = 1;
        PageSize = 15;
    }

    /// <summary>
    /// Instantiate a new page-able result to return 
    /// </summary>
    /// <param name="succeeded">Flags whether the process was successful</param>
    /// <param name="messages">The response messages</param>
    internal PaginatedResponse(bool succeeded, IEnumerable<string> messages)
        :base(succeeded, messages)
    {
        Data = Array.Empty<TData>();
        TotalCount = 0;
        CurrentPage = 1;
        PageSize = 15;
    }

    /// <summary>
    /// Instantiate a new page-able result to return 
    /// </summary>
    /// <param name="succeeded">Flags whether the process was successful</param>
    /// <param name="data">The content returned from the request</param>
    /// <param name="messages">The response messages</param>
    /// <param name="count">The total number of items</param>
    /// <param name="page">The current page number</param>
    /// <param name="pageSize">The number of items in a single page</param>
    internal PaginatedResponse(bool succeeded, IEnumerable<TData> data, int count, int page, int pageSize)
        :base(succeeded)
    {
        Data = data;
        TotalCount = count;
        CurrentPage = page;
        PageSize = pageSize;
    }

    /// <summary>
    /// Instantiate a new page-able result to return 
    /// </summary>
    /// <param name="succeeded">Flags whether the process was successful</param>
    /// <param name="data">The content returned from the request</param>
    /// <param name="messages">The response messages</param>
    /// <param name="count">The total number of items</param>
    /// <param name="page">The current page number</param>
    /// <param name="pageSize">The number of items in a single page</param>
    internal PaginatedResponse(bool succeeded, IEnumerable<string> messages, IEnumerable<TData> data, int count, int page, int pageSize)
        :base(succeeded, messages)
    {
        Data = data;
        TotalCount = count;
        CurrentPage = page;
        PageSize = pageSize;
    }

    /// <summary>
    /// A failed response
    /// </summary>
    /// <returns>An empty instance of <typeparamref name="TData"/></returns>
    public static PaginatedResponse<TData> Failure()
    {
        return new PaginatedResponse<TData>(false);
    }

    /// <summary>
    /// A failed response
    /// </summary>
    /// <param name="message">The error message to add</param>
    /// <returns>An empty instance of <typeparamref name="TData"/></returns>
    public static PaginatedResponse<TData> Failure(string message)
    {
        return new PaginatedResponse<TData>(false, new string[] { message });
    }

    /// <summary>
    /// A failed response
    /// </summary>
    /// <param name="messages">The error messages to add</param>
    /// <returns>An empty instance of <typeparamref name="TData"/></returns>
    public static PaginatedResponse<TData> Failure(IEnumerable<string> messages)
    {
        return new PaginatedResponse<TData>(false, messages);
    }

    /// <summary>
    /// A succcessful response
    /// </summary>
    /// <param name="data">The content returned from the request</param>
    /// <param name="count">The total number of items</param>
    /// <param name="page">The current page number</param>
    /// <param name="pageSize">The number of items in a single page</param>
    /// <returns>An instance of <typeparamref name="TData"/></returns>
    public static PaginatedResponse<TData> Success(IEnumerable<TData> data, int count, int page, int pageSize)
    {
        return new PaginatedResponse<TData>(true, data, count, page, pageSize);
    }

    /// <summary>
    /// The content returned from the request
    /// </summary>
    public IEnumerable<TData> Data { get; }

    /// <summary>
    /// The current page number
    /// </summary>
    public int CurrentPage { get; }

    /// <summary>
    /// The total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// The total number of items
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// The number of items in a single page
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Determines if there are  additional pages behind
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// Determines if there are additional pages ahead
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;
}
