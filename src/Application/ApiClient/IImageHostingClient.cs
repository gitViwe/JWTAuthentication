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
    /// <param name="expirationInSeconds">The expiration time in seconds</param>
    /// <returns>A <seealso cref="ImgBBUploadResponse"/> instance with the upload details</returns>
    Task<ImgBBUploadResponse> UploadImageAsync(IFormFile file, int? expirationInSeconds = null);

    /// <summary>
    /// Upload the image file
    /// </summary>
    /// <param name="httpContent">The content body to upload</param>
    /// <param name="fileName">The full name of the image file</param>
    /// <param name="expirationInSeconds">The expiration time in seconds</param>
    /// <returns>A <seealso cref="ImgBBUploadResponse"/> instance with the upload details</returns>
    Task<ImgBBUploadResponse> UploadImageAsync(HttpContent httpContent, string fileName, int? expirationInSeconds = null);
}
