using gitViwe.Shared.Utility;

namespace Application.Feature.Identity.UploadImage;

public class UploadImageRequestCommandValidator : AbstractValidator<UploadImageRequestCommand>
{
    public UploadImageRequestCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.File.Length)
            .LessThanOrEqualTo(5242880).WithMessage("The maximum allowed file size is " + Formatter.FormatSize(5242880));

        RuleFor(x => x.File.ContentType)
            .Equal("image/jpeg").WithMessage("Only image files with the extension [.jpg] are allowed.");
    }
}
