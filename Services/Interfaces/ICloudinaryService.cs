﻿namespace Lemoo_pos.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file, string path, string id);

        string GenerateImageId(string payload);
    }
}
