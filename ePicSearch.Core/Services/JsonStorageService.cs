using ePicSearch.Infrastructure.Entities;
using ePicSearch.Infrastructure.Entities.Interfaces;
using Newtonsoft.Json;

namespace ePicSearch.Infrastructure.Services
{
    public class JsonStorageService
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly string _jsonFilePath;

        public JsonStorageService(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
            _jsonFilePath = Path.Combine(_fileSystemService.GetAppDataDirectory(), "adventures.json");
        }

        public List<PhotoInfo> LoadAdventuresFromJson()
        {
            if (!_fileSystemService.FileExists(_jsonFilePath))
            {
                return new List<PhotoInfo>();
            }

            var json = _fileSystemService.ReadAllText(_jsonFilePath);
            return JsonConvert.DeserializeObject<List<PhotoInfo>>(json) ?? new List<PhotoInfo>();
        }

        public void SaveAdventuresToJson(List<PhotoInfo> adventures)
        {
            var json = JsonConvert.SerializeObject(adventures, Formatting.Indented);
            _fileSystemService.WriteAllText(_jsonFilePath, json);
        }
    }
}
