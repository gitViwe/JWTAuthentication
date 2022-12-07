using Shared.Constant;

namespace Shared.Exception;

/// <summary>
/// A custom identity exception
/// </summary>
public class HubIdentityException : System.Exception
{
    public HubIdentityException()
        : base(ErrorDescription.Generic.Identity) { }

    public HubIdentityException(string message)
        : base(message) { }

    public HubIdentityException(string message, System.Exception innerException)
        : base(message, innerException) { }
}
