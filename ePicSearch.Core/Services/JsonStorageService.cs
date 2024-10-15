using ePicSearch.Infrastructure.Entities;
using ePicSearch.Infrastructure.Entities.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ePicSearch.Infrastructure.Services
{
    public class JsonStorageService
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly ILogger<JsonStorageService> _logger;
        private readonly string _jsonFilePath;

        public JsonStorageService(IFileSystemService fileSystemService, ILogger<JsonStorageService> logger)
        {
            _fileSystemService = fileSystemService;
            _jsonFilePath = Path.Combine(_fileSystemService.GetAppDataDirectory(), "adventures.json");
            _logger = logger;

            _logger.LogInformation($"Initialized with file path: {_jsonFilePath}");
        }

        public List<PhotoInfo> LoadAdventuresFromJson()
        {
            _logger.LogInformation($"Loading adventures from JSON.");

            if (!_fileSystemService.FileExists(_jsonFilePath))
            {
                _logger.LogWarning($"JSON file not found. Returning empty adventure list.");
                return new List<PhotoInfo>();
            }

            try
            {
                var json = _fileSystemService.ReadAllText(_jsonFilePath);
                var adventures = JsonConvert.DeserializeObject<List<PhotoInfo>>(json) ?? new List<PhotoInfo>();
                _logger.LogInformation($"Successfully loaded {adventures.Count} adventures.");
                return adventures;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading adventures from JSON.");
                return new List<PhotoInfo>();
            }
        }

        public bool SaveAdventuresToJson(List<PhotoInfo> adventures)
        {
            _logger.LogInformation($"Saving {adventures.Count} adventures to JSON.");

            try
            {
                var json = JsonConvert.SerializeObject(adventures, Formatting.Indented);
                _fileSystemService.WriteAllText(_jsonFilePath, json);
                _logger.LogInformation($"Adventures successfully saved.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving adventures to JSON.");
                return false;
            }
        }

        public List<string> GetAllAdventureNames()
        {
            _logger.LogInformation($"Retrieving all adventure names.");

            var adventures = LoadAdventuresFromJson();
            var adventureNames = adventures
                .Select(p => p.AdventureName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct()
                .ToList();

            _logger.LogInformation($"Retrieved {adventureNames.Count} unique adventure names.");
            return adventureNames;
        }

        public List<PhotoInfo> GetPhotosForAdventure(string adventureName)
        {
            _logger.LogInformation($"Retrieving photos for adventure: {adventureName}");

            var adventures = LoadAdventuresFromJson();
            var photos = adventures.Where(p => p.AdventureName == adventureName).ToList();

            _logger.LogInformation($"Retrieved {photos.Count} photos for adventure: {adventureName}");
            return photos;
        }
    }
}
