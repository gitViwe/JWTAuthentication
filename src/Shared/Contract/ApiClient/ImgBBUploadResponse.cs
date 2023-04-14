using System.Text.Json.Serialization;

namespace Shared.Contract.ApiClient;

public class ImgBBUploadResponse
{
    [JsonPropertyName("data")]
    public ImgBBData Data { get; set; } = new();

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }
}

public class ImgBBData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("url_viewer")]
    public string UrlViewer { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("display_url")]
    public string DisplayUrl { get; set; } = string.Empty;

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("time")]
    public int Time { get; set; }

    [JsonPropertyName("expiration")]
    public int Expiration { get; set; }

    [JsonPropertyName("image")]
    public ImgBBImage Image { get; set; } = new();

    [JsonPropertyName("thumb")]
    public ImgBBThumb Thumb { get; set; } = new();

    [JsonPropertyName("delete_url")]
    public string DeleteUrl { get; set; } = string.Empty;
}

public class ImgBBImage
{
    [JsonPropertyName("filename")]
    public string Filename { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("mime")]
    public string Mime { get; set; } = string.Empty;

    [JsonPropertyName("extension")]
    public string Extension { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

public class ImgBBThumb
{
    [JsonPropertyName("filename")]
    public string Filename { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("mime")]
    public string Mime { get; set; } = string.Empty;

    [JsonPropertyName("extension")]
    public string Extension { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}
