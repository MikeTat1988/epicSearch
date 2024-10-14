using ePicSearch.Infrastructure.Entities;
using ePicSearch.Infrastructure.Entities.Interfaces;

namespace ePicSearch.Infrastructure.Services
{
    public class PhotoManager(PhotoStorageService photoStorageService, CodeGenerator codeGenerator, JsonStorageService jsonStorageService)
    {
        private readonly PhotoStorageService _photoStorageService = photoStorageService;
        private readonly CodeGenerator _codeGenerator = codeGenerator;
        private readonly JsonStorageService _jsonStorageService = jsonStorageService;

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

        public List<string> GetAllAdventureNames()
        {
            return _jsonStorageService.GetAllAdventureNames();
        }

        public List<PhotoInfo> GetPhotosForAdventure(string adventureName)
        {
            return _jsonStorageService.GetPhotosForAdventure(adventureName);
        }

        public void DeletePhoto(PhotoInfo photo)
        {
            bool isDeleted = PhotoStorageService.DeletePhoto(photo.FilePath);

            if (isDeleted)
            {
                RemovePhotoFromAdventure(photo.FilePath);
            }
        }

        private void AddPhotoToAdventure(PhotoInfo photoInfo)
        {
            var adventures = _jsonStorageService.LoadAdventuresFromJson();
            adventures.Add(photoInfo);
            _jsonStorageService.SaveAdventuresToJson(adventures);
        }

        private void RemovePhotoFromAdventure(string filePath)
        {
            var adventures = _jsonStorageService.LoadAdventuresFromJson();
            adventures.RemoveAll(p => p.FilePath == filePath);
            _jsonStorageService.SaveAdventuresToJson(adventures);
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
