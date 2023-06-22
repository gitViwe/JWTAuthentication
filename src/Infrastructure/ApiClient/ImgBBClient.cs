using Application.ApiClient;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OpenTelemetry.Trace;
using Shared.Contract.ApiClient;
using System.Diagnostics;

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

    public Task<ImgBBUploadResponse> UploadImageAsync(IFormFile file, int? expirationInSeconds = null)
    {
        return expirationInSeconds is not null
            ? UploadImageAsync(new StreamContent(file.OpenReadStream()), file.FileName, expirationInSeconds)
            : UploadImageAsync(new StreamContent(file.OpenReadStream()), file.FileName, int.Parse(_configuration[HubConfigurations.APIClient.ImgBB.Expiration]!));
    }

    public async Task<ImgBBUploadResponse> UploadImageAsync(HttpContent httpContent, string fileName, int? expirationInSeconds = null)
    {
        string endpoint = expirationInSeconds is not null
            ? UploadEndpoint + $"&expiration={expirationInSeconds}"
            : UploadEndpoint + $"&expiration={int.Parse(_configuration[HubConfigurations.APIClient.ImgBB.Expiration]!)}";

        var content = new MultipartFormDataContent
        {
            { httpContent, "image", fileName }
        };

        var result = await _httpClient.PostAsync(endpoint, content);

        try
        {
            result.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            using var activity = Activity.Current ?? default;
            activity?.RecordException(ex);
            throw;
        }

        return await result.ToResponseAsync<ImgBBUploadResponse>();
    }
}
