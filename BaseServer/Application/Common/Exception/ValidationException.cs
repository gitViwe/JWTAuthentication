namespace Application.Common.Exception;

internal class ValidationException : System.Exception
{
    internal ValidationException()
        : base(ErrorDescription.Validation.Generic)
    {
        ErrorMessages = Array.Empty<string>();
    }

    internal ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        ErrorMessages = failures.Select(x => x.ErrorMessage);
    }

    public IEnumerable<string> ErrorMessages { get; }
}
