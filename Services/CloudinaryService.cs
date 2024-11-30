using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Lemoo_pos.Services.Interfaces;

namespace Lemoo_pos.Services
{
    public class CloudinaryService : ICloudinaryService
    {

        private readonly Cloudinary _cloudinary;

        public CloudinaryService (Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }
        public async Task<string> UploadImageAsync(IFormFile file, string path)
        {
            if (file == null || file.Length <= 0)
            {
                throw new ArgumentException("The file is null or empty.");
            }

            try
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0; 

                var fileName = string.IsNullOrEmpty(file.FileName) ? "unknown" : file.FileName;

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, memoryStream),
                    Folder = "lemoo_pos/" + path
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK || uploadResult.Error != null)
                {
                    var errorMessage = uploadResult.Error?.Message ?? "Unknown error";
                    throw new Exception($"Cloudinary upload failed: {errorMessage}");
                }

                return uploadResult.SecureUrl.ToString();
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine($"ArgumentException: {ex.Message}");
                throw; 
            }
            catch (ObjectDisposedException ex)
            {
                Console.Error.WriteLine($"ObjectDisposedException: {ex.Message}");
                throw new Exception("Stream was already disposed before Cloudinary could process it.", ex);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Exception: {ex.Message}");
                throw new Exception("An error occurred while uploading the file to Cloudinary.", ex);
            }
        }

    }
}
