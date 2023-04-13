using Microsoft.AspNetCore.Http;

namespace Shared.Contract.Identity;

public class UploadImageRequest
{
    public IFormFile File { get; set; }
}
