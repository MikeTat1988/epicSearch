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

                // Append the correct file extension
                string fileExtension = Path.GetExtension(photo.FileName);
                string newFilePath = Path.Combine(adventureFolderPath, $"{photoInfo.Name}{fileExtension}");

                // Copy the photo from the original path to the adventure filder

                using var sourceStream = File.OpenRead(photo.FullPath);
                using var destinationStream = File.Create(newFilePath);

                await sourceStream.CopyToAsync(destinationStream);

                if (!DeletePhoto(photo.FullPath))
                {
                    _logger.LogWarning($"Failed to delete original photo: {photo.FullPath}");
                }

                return newFilePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to save photo: {ex.Message}");
                return string.Empty;
            }
        }

        public bool DeletePhoto(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        _logger.LogInformation($"Photo deleted: {filePath}");
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning($"Photo not found: {filePath}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error deleting photo: {ex.Message}");
                    return false;
                }
            }

            return false;
        }

        public async Task<DeleteFolderResult> DeleteAdventureFolderAsync(string adventureName)
        {
            string adventureFolderPath = Path.Combine(_appDataDirectory, adventureName);

            try
            {
                if (Directory.Exists(adventureFolderPath))
                {
                    _logger.LogInformation($"Deleting folder for adventure: {adventureName}");
                    await Task.Run(() => Directory.Delete(adventureFolderPath, true));   // Recursively delete contents
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

        public string GetPhotoPath(string fileName, string adventureName)
        {
            string adventureFolderPath = Path.Combine(_appDataDirectory, adventureName);
            string fullPath = Path.Combine(adventureFolderPath, fileName);

            if (File.Exists(fullPath))
            {
                return fullPath;
            }
            else
            {
                throw new FileNotFoundException($"Photo file not found: {fullPath}");
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
