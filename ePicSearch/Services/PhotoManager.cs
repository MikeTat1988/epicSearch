using ePicSearch.Infrastructure.Entities;
using ePicSearch.Infrastructure.Services;

namespace ePicSearch.Services
{
    public class PhotoManager(PhotoStorageServiceCore photoStorageService, CodeGenerator codeGenerator, JsonStorageService jsonStorageService)
    {
        private readonly PhotoStorageServiceCore _photoStorageService = photoStorageService;
        private readonly CodeGenerator _codeGenerator = codeGenerator;
        private readonly JsonStorageService _jsonStorageService = jsonStorageService;

        public async Task<PhotoInfo> CapturePhoto(IFileResult photo, string adventureName)
        {
            string photoCode = _codeGenerator.GenerateCode();
            int serialNumber = GetNextSerialNumber(adventureName);

            var photoInfo = new PhotoInfo
            {
                FilePath = photo.FullPath,
                Name = $"{photoCode}_{GetNextSerialNumber(adventureName)}",
                Code = photoCode,
                AdventureName = adventureName,
                SerialNumber = serialNumber
            }; 

            // Save photo to disk and update JSON file
            photoInfo.FilePath = await _photoStorageService.SavePhotoAsync(photo, photoInfo);
            SavePhotoToAdventure(photoInfo);

            return photoInfo;
        }

        public List<string> GetAllAdventureNames()
        {
            var adventures = _jsonStorageService.LoadAdventuresFromJson();
            return adventures
                .Select(p => p.AdventureName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct()
                .ToList();
        }

        public List<PhotoInfo> GetPhotosForAdventure(string adventureName)
        {
            var adventures = LoadAdventuresFromJson();
            return adventures.Where(p => p.AdventureName == adventureName).ToList();
        }

        public void DeletePhoto(PhotoInfo photo)
        {
            var adventures = LoadAdventuresFromJson();
            adventures.RemoveAll(p => p.FilePath == photo.FilePath);
            SaveAdventuresToJson(adventures);
        }

        private void SavePhotoToAdventure(PhotoInfo photoInfo)
        {
            var adventures = LoadAdventuresFromJson();
            adventures.Add(photoInfo);
            SaveAdventuresToJson(adventures);
        }

        private List<PhotoInfo> LoadAdventuresFromJson()
        {
            return _jsonStorageService.LoadAdventuresFromJson();
        }

        private void SaveAdventuresToJson(List<PhotoInfo> adventures)
        {
            _jsonStorageService.SaveAdventuresToJson(adventures);
        }

        private int GetNextSerialNumber(string adventureName)
        {
            var adventures = LoadAdventuresFromJson();
            return adventures.Where(p => p.AdventureName == adventureName).Select(p => p.SerialNumber).DefaultIfEmpty(0).Max() + 1;
        }
    }
}
