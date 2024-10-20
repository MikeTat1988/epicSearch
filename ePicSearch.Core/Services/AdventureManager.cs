using ePicSearch.Infrastructure.Entities;
using ePicSearch.Infrastructure.Entities.Interfaces;
using Microsoft.Extensions.Logging;

namespace ePicSearch.Infrastructure.Services
{
    public class AdventureManager(PhotoStorageService photoStorageService, 
        CodeGenerator codeGenerator, 
        DataStorageService dataStorageService, 
        ILogger<AdventureManager> logger)
    {
        private readonly PhotoStorageService _photoStorageService = photoStorageService;
        private readonly CodeGenerator _codeGenerator = codeGenerator;
        private readonly DataStorageService _dataStorageService = dataStorageService;
        private readonly ILogger<AdventureManager> _logger = logger;

        public async Task<PhotoInfo?> CapturePhoto(IFileResult photo, string adventureName)
        {
            try
            {
                string photoCode = _codeGenerator.GenerateCode();
                int serialNumber = GetNextSerialNumber(adventureName);

                var photoInfo = new PhotoInfo
                {
                    FilePath = photo.FullPath,
                    Name = $"{photoCode}_{serialNumber}",
                    Code = photoCode,
                    AdventureName = adventureName,
                    SerialNumber = serialNumber,
                    IsLocked = true
                };

                // Save photo 
                var newFilePath = await _photoStorageService.SavePhotoAsync(photo, photoInfo);

                if (string.IsNullOrEmpty(newFilePath))
                {
                    _logger.LogWarning($"Failed to save photo for adventure: {adventureName}");
                    return null;
                }

                photoInfo.FilePath = newFilePath;

                _dataStorageService.AddPhoto(photoInfo);

                _logger.LogInformation($"Photo captured and saved successfully for adventure: {adventureName}");

                return photoInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error capturing photo for adventure: {adventureName}");
                return null;
            }
        }

        public bool UpdatePhotoState(PhotoInfo updatedPhoto)
        {
            try
            {
                _dataStorageService.UpdatePhoto(updatedPhoto);

                _logger.LogInformation($"Photo state updated for photo: {updatedPhoto.FilePath}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating photo state for photo: {updatedPhoto.FilePath}");
                return false;
            }
        }

        public List<string> GetAllAdventureNames() => _dataStorageService.GetAllAdventureNames();

        public List<PhotoInfo> GetPhotosForAdventure(string adventureName) => _dataStorageService.GetPhotosForAdventure(adventureName);

        public async Task<bool> DeleteAdventureAsync(string adventureName)
        {
            try
            {
                // Delete photos from storage
                var deleteResult = await _photoStorageService.DeleteAllPhotosForAdventureAsync(adventureName);

                if (deleteResult == PhotoStorageService.DeleteFolderResult.Failure)
                {
                    _logger.LogWarning($"Failed to delete photos for adventure: {adventureName}");
                    return false;
                }

                // Remove adventure from data storage
                _dataStorageService.RemoveAdventure(adventureName);

                _logger.LogInformation($"Adventure deleted successfully: {adventureName}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting adventure: {adventureName}");
                return false;
            }
        }

        private int GetNextSerialNumber(string adventureName)
        {
            var photos = _dataStorageService.GetPhotosForAdventure(adventureName);
            return photos.Select(p => p.SerialNumber).DefaultIfEmpty(0).Max() + 1;
        }
    }
}
