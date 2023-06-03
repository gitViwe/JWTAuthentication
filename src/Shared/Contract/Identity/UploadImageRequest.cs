using Microsoft.AspNetCore.Http;

namespace Shared.Contract.Identity;

public class UploadImageRequest
{
    public required IFormFile File { get; set; }
}
