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

        public async Task<PhotoInfo> CapturePhoto(IFileResult photo, string adventureName)
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

            // Save photo to disk and update JSON file
            photoInfo.FilePath = await _photoStorageService.SavePhotoAsync(photo, photoInfo);
            AddPhotoToAdventure(photoInfo);

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
                RemovePhotosFromJson(photos);
                return true;
            }
            return false;
        }

        private void AddPhotoToAdventure(PhotoInfo photoInfo)
        {
            var adventures = _jsonStorageService.LoadAdventuresFromJson();
            adventures.Add(photoInfo);
            SaveAdventuresToJson(adventures);
            _logger.LogInformation($"Added photo to adventure: {photoInfo.AdventureName}");
        }

        private void RemovePhotosFromJson(List<PhotoInfo> photos)
        {
            var allPhotos = _jsonStorageService.LoadAdventuresFromJson();
            allPhotos.RemoveAll(p => photos.Any(photo => photo.FilePath == p.FilePath));
            SaveAdventuresToJson(allPhotos);
            _logger.LogInformation("Adventure photos removed from JSON and synced.");
        }

        private void SaveAdventuresToJson(List<PhotoInfo> adventures)
        {
            _jsonStorageService.SaveAdventuresToJson(adventures);
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
