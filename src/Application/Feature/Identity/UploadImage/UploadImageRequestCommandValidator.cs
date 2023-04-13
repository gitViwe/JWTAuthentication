using gitViwe.Shared.Utility;

namespace Application.Feature.Identity.UploadImage;

public class UploadImageRequestCommandValidator : AbstractValidator<UploadImageRequestCommand>
{
    public UploadImageRequestCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.File.Length)
            .LessThanOrEqualTo(5242880).WithMessage("The maximum allowed file size is " + FileSizeFormatter.FormatSize(5242880));
    }
}
