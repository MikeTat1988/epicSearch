using ePicSearch.Infrastructure.Entities;
using ePicSearch.Infrastructure.Entities.Interfaces;
using Microsoft.Extensions.Logging;
using static ePicSearch.Infrastructure.Services.PhotoStorageService;

namespace ePicSearch.Infrastructure.Services
{
    public class PhotoManager(PhotoStorageService photoStorageService, 
        CodeGenerator codeGenerator, 
        JsonStorageService jsonStorageService, 
        ILogger<PhotoManager> logger)
    {
        private readonly PhotoStorageService _photoStorageService = photoStorageService;
        private readonly CodeGenerator _codeGenerator = codeGenerator;
        private readonly JsonStorageService _jsonStorageService = jsonStorageService;
        private readonly ILogger<PhotoManager> _logger = logger;

        public async Task<PhotoInfo?> CapturePhoto(IFileResult photo, string adventureName)
        {
            string photoCode = _codeGenerator.GenerateCode();
            int serialNumber = GetNextAvailableSerialNumberForAdventure(adventureName);

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

            if (!AddPhotoToAdventure(photoInfo))
            {
                _logger.LogWarning($"Failed to update JSON for adventure: {adventureName}");
                return null; // Return null if JSON update fails
            }

            _logger.LogInformation($"Photo captured and saved successfully for {adventureName}");
            return photoInfo;
        }

        public bool UpdatePhotoState(PhotoInfo updatedPhoto)
        {
            var allPhotos = _jsonStorageService.LoadAllAdventures();

            _logger.LogInformation($"Updating photo state for {updatedPhoto.FilePath}");

            // Find the existing photo and update its state
            var photoToUpdate = allPhotos.FirstOrDefault(p => p.FilePath == updatedPhoto.FilePath);
            if (photoToUpdate != null)
            {
                photoToUpdate.IsLocked = updatedPhoto.IsLocked;

                // Save the updated list back to the cache
                _jsonStorageService.SaveAllAdventures(allPhotos);

                _logger.LogInformation($"Updated photo state for {updatedPhoto.FilePath}");

                return true;
            }
            else
            {
                _logger.LogWarning($"Could not find the photo to update: {updatedPhoto.FilePath}");
                return false;
            }
        }

        public List<string> GetAllAdventureNames() => _jsonStorageService.GetAllAdventureNames();

        public List<PhotoInfo> GetPhotosForAdventure(string adventureName) => _jsonStorageService.GetPhotosForAdventure(adventureName);

        public async Task<bool> DeleteAdventureAsync(string adventureName)
        {
            try
            {
                var photos = GetPhotosForAdventure(adventureName);
                _logger.LogInformation($"Loaded photos : \n {string.Join(",\n", photos)} for  {adventureName}");

                var deleteResult = await _photoStorageService.DeleteAdventureFolderAsync(adventureName);

                if (deleteResult == DeleteFolderResult.Failure)
                {
                    _logger.LogWarning($"Failed to delete adventure folder for: {adventureName} due to an error.");
                    return false;
                }

                if (deleteResult == DeleteFolderResult.NotFound)
                {
                    _logger.LogWarning($"Adventure folder not found: {adventureName}. Proceeding with JSON cleanup.");
                }

                // Remove photos from JSON cache
                if (!RemovePhotosFromJson(photos))
                {
                    _logger.LogWarning($"Failed to update JSON after deleting adventure: {adventureName}");
                    return false; // Exit if JSON update fails
                }

                // Ensure the cache syncs to file only if everything else succeeded
                _jsonStorageService.SyncCacheToFile();
                _logger.LogInformation($"Successfully deleted adventure: {adventureName}");

                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting adventure: {adventureName}");
            }

            return false;
        }

        private bool AddPhotoToAdventure(PhotoInfo photoInfo)
        {
            var adventures = _jsonStorageService.LoadAllAdventures();
            adventures.Add(photoInfo);
            return _jsonStorageService.SaveAllAdventures(adventures);
        }

        private bool RemovePhotosFromJson(List<PhotoInfo> photos)
        {
            var allPhotos = _jsonStorageService.LoadAllAdventures();
            allPhotos.RemoveAll(p => photos.Any(photo => photo.FilePath == p.FilePath));

            if(_jsonStorageService.SaveAllAdventures(allPhotos))
            {
                _logger.LogInformation("Adventure photos removed from JSON and synced.");
                return true;
            }

            _logger.LogError("Failed to update JSON after removing adventure photos.");
            return false;
        }

        private int GetNextAvailableSerialNumberForAdventure(string adventureName)
        {
            var adventures = _jsonStorageService.LoadAllAdventures();
            return adventures
                .Where(p => p.AdventureName == adventureName)
                .Select(p => p.SerialNumber)
                .DefaultIfEmpty(0)
                .Max() + 1;
        }
    }
}
