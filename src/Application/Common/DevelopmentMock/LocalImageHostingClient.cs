﻿using Application.ApiClient;
using Microsoft.AspNetCore.Http;
using Shared.Contract.ApiClient;

namespace Application.Common.DevelopmentMock;

public class LocalImageHostingClient : IImageHostingClient
{
    private readonly string _uploadFolderPath;

    public LocalImageHostingClient(string uploadFolderPath)
    {
        _uploadFolderPath = uploadFolderPath;
    }

    public async Task<ImgBBUploadResponse> UploadImageAsync(IFormFile file)
    {
        var uniqueFileName = Generator.RandomString(length: 6) + Path.GetExtension(file.FileName);

        var filePath = Path.Combine(_uploadFolderPath, uniqueFileName);

        if (!Directory.Exists(_uploadFolderPath))
        {
            Directory.CreateDirectory(_uploadFolderPath);
        }

        // Create a new FileStream to write the uploaded file to the local server folder
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var response = new ImgBBUploadResponse
        {
            Success = true,
            Status = 200,
            Data = new()
            {
                Id = "Pr7TkcH",
                Title = "19s-by-wlop-dc6nyxz",
                UrlViewer = "https://ibb.co/Pr7TkcH",
                Url = filePath,
                DisplayUrl = filePath,
                Width = 500,
                Height = 500,
                Size = 1057,
                Time = 1681544384,
                Expiration = 1681544384,
                DeleteUrl = "https://ibb.co/Pr7TkcH/6f41969f415bd35a5730bb9da29ec623",
                Image = new()
                {
                    Filename = file.FileName,
                    Name = file.FileName.Split('.').FirstOrDefault()!,
                    Mime = file.ContentType,
                    Extension = file.FileName.Split('.').LastOrDefault()!,
                    Url = filePath
                },
                Thumb = new()
                {
                    Filename = file.FileName,
                    Name = file.FileName.Split('.').FirstOrDefault()!,
                    Mime = file.ContentType,
                    Extension = file.FileName.Split('.').LastOrDefault()!,
                    Url = filePath
                }
            }
        };

        return response;
    }
}
