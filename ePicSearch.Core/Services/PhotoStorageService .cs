using ePicSearch.Infrastructure.Entities;
using ePicSearch.Infrastructure.Entities.Interfaces;
using Microsoft.Extensions.Logging;

namespace ePicSearch.Infrastructure.Services
{
    public class PhotoStorageService(IFileSystemService fileSystemService, ILogger<PhotoStorageService> logger)
    {
        private readonly string _appDataDirectory = fileSystemService.GetAppDataDirectory();
        private readonly ILogger<PhotoStorageService> _logger = logger;

        public async Task<string> SavePhotoAsync(IFileResult photo, PhotoInfo photoInfo)
        {
            try
            {
                string adventureFolderPath = Path.Combine(_appDataDirectory, photoInfo.AdventureName);

                if (!Directory.Exists(adventureFolderPath))
                {
                    Directory.CreateDirectory(adventureFolderPath);
                }

                string fileExtension = Path.GetExtension(photo.FileName);
                string newFilePath = Path.Combine(adventureFolderPath, $"{photoInfo.Name}{fileExtension}");

                // Copy the photo from the original path to the adventure filder
                using var sourceStream = File.OpenRead(photo.FullPath);
                using var destinationStream = File.Create(newFilePath);

                await sourceStream.CopyToAsync(destinationStream);

                DeletePhoto(photo.FullPath);

                _logger.LogInformation($"Photo saved to {newFilePath}");

                return newFilePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to save photo: {ex.Message}");
                return string.Empty;
            }
        }

        public async Task<DeleteFolderResult> DeleteAllPhotosForAdventureAsync(string adventureName)
        {
            string adventureFolderPath = Path.Combine(_appDataDirectory, adventureName);

            try
            {
                if (Directory.Exists(adventureFolderPath))
                {
                    _logger.LogInformation($"Deleting folder for adventure: {adventureName}");

                    Directory.Delete(adventureFolderPath, true); 

                    return DeleteFolderResult.Success;
                }
                else
                {
                    _logger.LogWarning($"Adventure folder not found: {adventureName}");
                    return DeleteFolderResult.NotFound;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting folder for adventure: {adventureName}");
                return DeleteFolderResult.Failure;
            }
        }

        private void DeletePhoto(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation($"Deleted original photo: {filePath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting original photo: {filePath}");
            }
        }

        public enum DeleteFolderResult
        {
            Success,
            NotFound,
            Failure
        }
    }
}
