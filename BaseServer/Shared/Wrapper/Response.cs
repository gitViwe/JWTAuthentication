namespace Shared.Wrapper;

/// <summary>
/// A unified return type for the API endpoint
/// </summary>
public class Response : IResponse
{
    /// <summary>
    /// Instantiate a new response to return
    /// </summary>
    /// <param name="succeeded">Flags whether the process was successful</param>
    internal Response(bool succeeded)
    {
        Succeeded = succeeded;
        Messages = Array.Empty<string>();
    }

    /// <summary>
    /// Instantiate a new response to return
    /// </summary>
    /// <param name="succeeded">Flags whether the process was successful</param>
    /// <param name="messages">The response messages</param>
    internal Response(bool succeeded, IEnumerable<string> messages)
    {
        Succeeded = succeeded;
        Messages = messages;
    }

    /// <summary>
    /// Flags whether the process was successful
    /// </summary>
    public bool Succeeded { get; }

    /// <summary>
    /// The response messages
    /// </summary>
    public IEnumerable<string> Messages { get; }

    /// <summary>
    /// Unsuccessful result
    /// </summary>
    /// <returns>The <see cref="Succeeded"/> property value</returns>
    public static IResponse Fail()
    {
        return new Response(false);
    }

    /// <summary>
    /// Unsuccessful result
    /// </summary>
    /// <param name="message">The error message to add</param>
    /// <returns>The <see cref="Succeeded"/> and <see cref="Messages"/> property values</returns>
    public static IResponse Fail(string message)
    {
        return new Response(false, new string[] { message });
    }

    /// <summary>
    /// Unsuccessful result
    /// </summary>
    /// <param name="messages">The error messages to add</param>
    /// <returns>The <see cref="Succeeded"/> and <see cref="Messages"/> property values</returns>
    public static IResponse Fail(IEnumerable<string> messages)
    {
        return new Response(false, messages);
    }

    /// <summary>
    /// Successful result
    /// </summary>
    /// <returns>The <see cref="Succeeded"/> property value</returns>
    public static IResponse Success()
    {
        return new Response(true);
    }

    /// <summary>
    /// Successful result
    /// </summary>
    /// <param name="message">The success message to add</param>
    /// <returns>The <see cref="Succeeded"/> and <see cref="Messages"/> property values</returns>
    public static IResponse Success(string message)
    {
        return new Response(true, new string[] { message });
    }

    /// <summary>
    /// Successful result
    /// </summary>
    /// <param name="messages">The error messages to add</param>
    /// <returns>The <see cref="Succeeded"/> and <see cref="Messages"/> property values</returns>
    public static IResponse Success(IEnumerable<string> messages)
    {
        return new Response(true, messages);
    }
}

/// <summary>
/// Extends on <see cref="Response"/> to return data
/// </summary>
/// <typeparam name="TData">The data type returned from the request</typeparam>
public class Response<TData> : IResponse<TData> where TData : class, new()
{
    /// <summary>
    /// Instantiate a new response to return
    /// </summary>
    /// <param name="succeeded">Flags whether the process was successful</param>
    /// <param name="data">The content returned from the request</param>
    internal Response(bool succeeded)
    {
        Succeeded = succeeded;
        Messages = Array.Empty<string>();
        Data = new();
    }

    /// <summary>
    /// Instantiate a new response to return
    /// </summary>
    /// <param name="succeeded">Flags whether the process was successful</param>
    /// <param name="data">The content returned from the request</param>
    internal Response(bool succeeded, TData data)
    {
        Succeeded = succeeded;
        Messages = Array.Empty<string>();
        Data = data;
    }

    /// <summary>
    /// Instantiate a new response to return
    /// </summary>
    /// <param name="succeeded">Flags whether the process was successful</param>
    /// <param name="messages">The response messages</param>
    internal Response(bool succeeded, IEnumerable<string> messages)
    {
        Succeeded = succeeded;
        Messages = messages;
        Data = new();
    }

    /// <summary>
    /// Instantiate a new response to return
    /// </summary>
    /// <param name="succeeded">Flags whether the process was successful</param>
    /// <param name="messages">The response messages</param>
    /// <param name="data">The content returned from the request</param>
    internal Response(bool succeeded, IEnumerable<string> messages, TData data)
    {
        Succeeded = succeeded;
        Messages = messages;
        Data = data;
    }

    /// <summary>
    /// Flags whether the process was successful
    /// </summary>
    public bool Succeeded { get; }

    /// <summary>
    /// The response messages
    /// </summary>
    public IEnumerable<string> Messages { get; }

    /// <summary>
    /// The content returned from the request
    /// </summary>
    public TData Data { get; }

    /// <summary>
    /// Unsuccessful result
    /// </summary>
    /// <returns>The <see cref="Succeeded"/> property value</returns>
    public static IResponse<TData> Fail()
    {
        return new Response<TData>(false);
    }

    /// <summary>
    /// Unsuccessful result
    /// </summary>
    /// <param name="message">The error message to add</param>
    /// <returns>The <see cref="Succeeded"/> and <see cref="Messages"/> property values</returns>
    public static IResponse<TData> Fail(string message)
    {
        return new Response<TData>(false, new string[] { message });
    }

    /// <summary>
    /// Unsuccessful result
    /// </summary>
    /// <param name="messages">The error messages to add</param>
    /// <returns>The <see cref="Succeeded"/> and <see cref="Messages"/> property values</returns>
    public static IResponse<TData> Fail(IEnumerable<string> messages)
    {
        return new Response<TData>(false, messages);
    }

    /// <summary>
    /// Successful result
    /// </summary>
    /// <param name="data">The content to add</param>
    /// <returns>The <see cref="Data"/> property value</returns>
    public static IResponse<TData> Success(TData data)
    {
        return new Response<TData>(true, data);
    }

    /// <summary>
    /// Successful result
    /// </summary>
    /// <param name="message">The success message to add</param>
    /// <param name="data">The content to add</param>
    /// <returns>The <see cref="Succeeded"/> and <see cref="Messages"/> property values</returns>
    public static IResponse<TData> Success(string message, TData data)
    {
        return new Response<TData>(true, new string[] { message }, data);
    }

    /// <summary>
    /// Successful result
    /// </summary>
    /// <param name="messages">The success messages to add</param>
    /// <param name="data">The content to add</param>
    /// <returns>The <see cref="Data"/> and <see cref="Messages"/> property values</returns>
    public static IResponse<TData> Success(IEnumerable<string> messages, TData data)
    {
        return new Response<TData>(true, messages, data);
    }
}
