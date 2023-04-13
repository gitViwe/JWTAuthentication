using Application.Common.ApiClient;
using gitViwe.Shared.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Shared.Constant;
using Shared.Contract.ApiClient;

namespace Infrastructure.ApiClient;

internal class ImgBBClient : IImageHostingClient
{
    private readonly HttpClient _httpClient;
    private string UploadEndpoint { get; }

    public ImgBBClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        UploadEndpoint = $"1/upload" +
            $"?expiration=6{configuration[HubConfigurations.APIClient.ImgBB.Expiration]}" +
            $"&key={configuration[HubConfigurations.APIClient.ImgBB.APIKey]}";
    }

    public async Task<ImgBBUploadResponse> UploadImageAsync(IFormFile file)
    {
        var content = new MultipartFormDataContent
        {
            { new StreamContent(file.OpenReadStream()), "image", file.FileName }
        };

        var result = await _httpClient.PostAsync(UploadEndpoint, content);

        result.EnsureSuccessStatusCode();

        return await result.ToResponseAsync<ImgBBUploadResponse>();
    }
}
