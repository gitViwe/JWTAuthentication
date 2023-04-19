using Application.ApiClient;
using gitViwe.Shared.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Shared.Constant;
using Shared.Contract.ApiClient;

namespace Infrastructure.ApiClient;

internal class ImgBBClient : IImageHostingClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private string UploadEndpoint { get; }

    public ImgBBClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        UploadEndpoint = $"1/upload?key={configuration[HubConfigurations.APIClient.ImgBB.APIKey]}";
    }

    public async Task<ImgBBUploadResponse> UploadImageAsync(IFormFile file)
    {
        string endpoint = UploadEndpoint + $"&expiration={_configuration[HubConfigurations.APIClient.ImgBB.Expiration]}";
        var content = new MultipartFormDataContent
        {
            { new StreamContent(file.OpenReadStream()), "image", file.FileName }
        };

        var result = await _httpClient.PostAsync(endpoint, content);

        result.EnsureSuccessStatusCode();

        return await result.ToResponseAsync<ImgBBUploadResponse>();
    }

    public async Task<ImgBBUploadResponse> UploadImageAsync(HttpContent httpContent, string fileName, int expirationInSeconds = 300)
    {
        string endpoint = UploadEndpoint + $"&expiration={expirationInSeconds}";
        var content = new MultipartFormDataContent
        {
            { httpContent, "image", fileName }
        };

        var result = await _httpClient.PostAsync(endpoint, content);

        result.EnsureSuccessStatusCode();

        return await result.ToResponseAsync<ImgBBUploadResponse>();
    }
}
