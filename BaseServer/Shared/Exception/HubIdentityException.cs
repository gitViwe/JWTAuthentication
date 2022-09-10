using Shared.Constant;

namespace Shared.Exception;

/// <summary>
/// A custom identity exception
/// </summary>
public class HubIdentityException : System.Exception
{
    public HubIdentityException()
        : base(ErrorDescription.Generic.Identity) { }
}
