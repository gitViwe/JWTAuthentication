using Application.ApiClient;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Shared.Contract.ApiClient;

namespace Infrastructure.ApiClient;

internal class ImgBBClient : IImageHostingClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _uploadEndpoint;

    public ImgBBClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _uploadEndpoint = $"1/upload?key={configuration[HubConfigurations.APIClient.ImgBB.APIKey]}";
    }

    public Task<ImgBBUploadResponse> UploadImageAsync(IFormFile file, int? expirationInSeconds = null)
    {
        return expirationInSeconds is not null
            ? UploadImageAsync(new StreamContent(file.OpenReadStream()), file.FileName, expirationInSeconds)
            : UploadImageAsync(new StreamContent(file.OpenReadStream()), file.FileName, int.Parse(_configuration[HubConfigurations.APIClient.ImgBB.Expiration]!));
    }

    public async Task<ImgBBUploadResponse> UploadImageAsync(HttpContent httpContent, string fileName, int? expirationInSeconds = null)
    {
        string endpoint = expirationInSeconds is not null
            ? _uploadEndpoint + $"&expiration={expirationInSeconds}"
            : _uploadEndpoint + $"&expiration={int.Parse(_configuration[HubConfigurations.APIClient.ImgBB.Expiration]!)}";

        var content = new MultipartFormDataContent
        {
            { httpContent, "image", fileName }
        };

        var result = await _httpClient.PostAsync(endpoint, content);

        result.EnsureSuccessStatusCode();

        return await result.ToResponseAsync<ImgBBUploadResponse>();
    }
}
