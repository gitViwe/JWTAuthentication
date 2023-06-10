using Application.ApiClient;
using Microsoft.AspNetCore.Http;
using Shared.Contract.ApiClient;

namespace Infrastructure.ApiClient;

internal class LocalMockClient : IImageHostingClient
{
    public async Task<ImgBBUploadResponse> UploadImageAsync(IFormFile file, int? expirationInSeconds = null)
    {
        string base64 = await GetBase64StringFromFormFileAsync(file);

        return new ImgBBUploadResponse()
        {
            Status = 200,
            Success = true,
            Data = new ImgBBData()
            {
                DisplayUrl = base64,
                Image = new()
                {
                    Filename = file.Name,
                    Name = file.Name,
                    Url = base64,
                },
                Thumb = new()
                {
                    Filename = file.Name,
                    Name = file.Name,
                    Url = base64,
                }
            }
        };
    }

    public async Task<ImgBBUploadResponse> UploadImageAsync(HttpContent httpContent, string fileName, int? expirationInSeconds = null)
    {
        string base64 = await GetBase64StringFromHttpContentAsync(httpContent);

        return new ImgBBUploadResponse()
        {
            Status = 200,
            Success = true,
            Data = new ImgBBData()
            {
                DisplayUrl = base64,
                Image = new()
                {
                    Filename = fileName,
                    Name = fileName,
                    Url = base64,
                },
                Thumb = new()
                {
                    Filename = fileName,
                    Name = fileName,
                    Url = base64,
                }
            }
        };
    }

    private static async Task<string> GetBase64StringFromFormFileAsync(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        byte[] fileBytes = memoryStream.ToArray();
        string base64String = Convert.ToBase64String(fileBytes);

        return $"data:{file.ContentType};base64," + base64String;
    }

    private static async Task<string> GetBase64StringFromHttpContentAsync(HttpContent httpContent)
    {
        using var memoryStream = new MemoryStream();
        await httpContent.CopyToAsync(memoryStream);

        byte[] fileBytes = memoryStream.ToArray();
        string base64String = Convert.ToBase64String(fileBytes);

        return $"data:{httpContent.Headers?.ContentType?.MediaType};base64," + base64String;
    }
}
