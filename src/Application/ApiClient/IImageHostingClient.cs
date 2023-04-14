using Microsoft.AspNetCore.Http;
using Shared.Contract.ApiClient;

namespace Application.ApiClient;

/// <summary>
/// An abstraction of an image hosting service
/// </summary>
public interface IImageHostingClient
{
    /// <summary>
    /// Upload the image file.
    /// </summary>
    /// <param name="file">The form file to upload</param>
    /// <returns>A <seealso cref="ImgBBUploadResponse"/> instance with the upload details</returns>
    Task<ImgBBUploadResponse> UploadImageAsync(IFormFile file);
}
