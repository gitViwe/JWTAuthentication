using FluentValidation.Results;
using Shared.Constant;

namespace Shared.Exception;

/// <summary>
/// A custom validation exception
/// </summary>
public class HubValidationException : System.Exception
{
    public HubValidationException()
        : base(ErrorDescription.Generic.Validation)
    {
        ErrorMessages = Array.Empty<string>();
    }

    public HubValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        ErrorMessages = failures.Select(x => x.ErrorMessage);
    }

    /// <summary>
    /// A collection of the validation errors
    /// </summary>
    public IEnumerable<string> ErrorMessages { get; }
}
