using ePicSearch.Infrastructure.Entities;
using ePicSearch.Infrastructure.Entities.Interfaces;
using Microsoft.Extensions.Logging;

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
                SerialNumber = serialNumber
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

        public List<string> GetAllAdventureNames() => _jsonStorageService.GetAllAdventureNames();

        public List<PhotoInfo> GetPhotosForAdventure(string adventureName) => _jsonStorageService.GetPhotosForAdventure(adventureName);

        public bool DeleteAdventure(string adventureName)
        {
            var photos = GetPhotosForAdventure(adventureName);
            bool folderDeleted = _photoStorageService.DeleteAdventureFolder(adventureName);

            if (folderDeleted)
            {
                if (RemovePhotosFromJson(photos))
                {
                    _logger.LogInformation($"Successfully deleted adventure: {adventureName}");
                    return true;
                }
                else
                {
                    _logger.LogWarning($"Failed to update JSON after deleting adventure: {adventureName}");
                }
            }

            _logger.LogWarning($"Failed to delete adventure folder for: {adventureName}");
            return false;
        }

        private bool AddPhotoToAdventure(PhotoInfo photoInfo)
        {
            var adventures = _jsonStorageService.LoadAdventuresFromJson();
            adventures.Add(photoInfo);
            return _jsonStorageService.SaveAdventuresToJson(adventures);
        }

        private bool RemovePhotosFromJson(List<PhotoInfo> photos)
        {
            var allPhotos = _jsonStorageService.LoadAdventuresFromJson();
            allPhotos.RemoveAll(p => photos.Any(photo => photo.FilePath == p.FilePath));

            if(_jsonStorageService.SaveAdventuresToJson(allPhotos))
            {
                _logger.LogInformation("Adventure photos removed from JSON and synced.");
                return true;
            }

            _logger.LogError("Failed to update JSON after removing adventure photos.");
            return false;
        }

        private int GetNextAvailableSerialNumberForAdventure(string adventureName)
        {
            var adventures = _jsonStorageService.LoadAdventuresFromJson();
            return adventures
                .Where(p => p.AdventureName == adventureName)
                .Select(p => p.SerialNumber)
                .DefaultIfEmpty(0)
                .Max() + 1;
        }
    }
}
