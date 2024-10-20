using ePicSearch.Infrastructure.Entities;
using ePicSearch.Infrastructure.Entities.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ePicSearch.Infrastructure.Services
{
    public class DataStorageService
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly ILogger<DataStorageService> _logger;
        private readonly string _jsonFilePath;
        private readonly object _cacheLock = new object();

        private List<PhotoInfo> _cache;
        private bool _isCacheDirty = false;

        public DataStorageService(IFileSystemService fileSystemService, ILogger<DataStorageService> logger)
        {
            _fileSystemService = fileSystemService;
            _jsonFilePath = Path.Combine(_fileSystemService.GetAppDataDirectory(), "adventures.json");
            _logger = logger;

            _logger.LogInformation($"Initialized with file path: {_jsonFilePath}");
            _cache = LoadAdventuresFromFile();
        }

        public List<PhotoInfo> LoadAdventuresFromFile()
        {
            lock (_cacheLock)
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
                    _logger.LogInformation($"Loaded {adventures.Count} adventures from JSON.");
                    return adventures;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error loading adventures from JSON.");
                    return new List<PhotoInfo>();
                }
            }
        }

        public void RemoveAdventure(string adventureName)
        {
            lock (_cacheLock)
            {
                _cache.RemoveAll(p => p.AdventureName == adventureName);
                _isCacheDirty = true;
            }
        }

        public List<PhotoInfo> GetPhotosForAdventure(string adventureName)
        {
            lock (_cacheLock)
            {
                return _cache.Where(p => p.AdventureName == adventureName).ToList();
            }
        }

        public void AddPhoto(PhotoInfo photoInfo)
        {
            lock (_cacheLock)
            {
                _cache.Add(photoInfo);
                _isCacheDirty = true;
            }
        }

        public void UpdatePhoto(PhotoInfo updatedPhoto)
        {
            lock (_cacheLock)
            {
                var existingPhoto = _cache.FirstOrDefault(p => p.FilePath == updatedPhoto.FilePath);
                if (existingPhoto != null)
                {
                    existingPhoto.IsLocked = updatedPhoto.IsLocked;
                    _isCacheDirty = true;
                }
            }
        }

        public List<string> GetAllAdventureNames()
        {
            lock (_cacheLock)
            {
                return _cache.Select(p => p.AdventureName).Distinct().ToList();
            }
        }

        public void SyncCacheToFile()
        {
            lock (_cacheLock)
            {
                if (!_isCacheDirty)
                {
                    _logger.LogInformation("Cache is clean. No need to sync with JSON.");
                    return;
                }

                try
                {
                    _logger.LogInformation("Syncing cache with JSON file.");
                    _logger.LogInformation($"cache written : \n {string.Join(",\n", _cache)}");
                    
                    var json = JsonConvert.SerializeObject(_cache, Formatting.Indented);
                    _fileSystemService.WriteAllText(_jsonFilePath, json);
                    _isCacheDirty = false;  
                    _logger.LogInformation("Cache successfully synced to JSON.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error syncing cache to JSON.");
                }
            }
        }
    }
}
