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

        private List<PhotoInfo> _photoCache;
        private List<AdventureData> _adventureCache;
        private bool _isCacheDirty = false;

        public DataStorageService(IFileSystemService fileSystemService, ILogger<DataStorageService> logger)
        {
            _fileSystemService = fileSystemService;
            _jsonFilePath = Path.Combine(_fileSystemService.GetAppDataDirectory(), "adventures.json");
            _logger = logger;

            _logger.LogInformation($"Initialized with file path: {_jsonFilePath}");
            _photoCache = LoadFromFile<PhotoInfo>();
            _adventureCache = LoadFromFile<AdventureData>();
        }

        public List<T> LoadFromFile<T>() where T : new()
        {
            lock (_cacheLock)
            {
                _logger.LogInformation($"Loading {typeof(T).Name} from JSON.");

                if (!_fileSystemService.FileExists(_jsonFilePath))
                {
                    _logger.LogWarning($"JSON file not found for  {typeof(T).Name}. Returning empty adventure list.");
                    return new List<T>();
                }

                try
                {
                    var json = _fileSystemService.ReadAllText(_jsonFilePath);
                    var data = JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
                    _logger.LogInformation($"Loaded {data.Count} items of type {typeof(T).Name} from JSON.");
                    return data;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error loading {typeof(T).Name} data from JSON.");
                    return new List<T>();
                }
            }
        }

        public void RemoveAdventure(string adventureName)
        {
            lock (_cacheLock)
            {
                _photoCache.RemoveAll(p => p.AdventureName == adventureName);
                _isCacheDirty = true;
            }
        }

        public List<PhotoInfo> GetPhotosForAdventure(string adventureName)
        {
            lock (_cacheLock)
            {
                return _photoCache.Where(p => p.AdventureName == adventureName).ToList();
            }
        }

        public AdventureData? GetAdventureData(string adventureName)
        {
            lock (_cacheLock)
            {
                return _adventureCache.FirstOrDefault(a => a.AdventureName == adventureName);
            }
        }

        public void AddPhoto(PhotoInfo photoInfo)
        {
            lock (_cacheLock)
            {
                _photoCache.Add(photoInfo);
                _isCacheDirty = true;
            }
        }

        public void AddAdventure(AdventureData adventureData)
        {
            lock (_cacheLock)
            {
                _adventureCache.Add(adventureData);
                _isCacheDirty = true;
            }
        }

        public void UpdatePhoto(PhotoInfo updatedPhoto)
        {
            lock (_cacheLock)
            {
                var existingPhoto = _photoCache.FirstOrDefault(p => p.FilePath == updatedPhoto.FilePath);
                if (existingPhoto != null)
                {
                    existingPhoto.IsLocked = updatedPhoto.IsLocked;
                    _isCacheDirty = true;
                }
            }
        }

        public void UpdateAdventureData(AdventureData updatedAdventure)
        {
            lock (_cacheLock)
            {
                var existingAdventure = _adventureCache.FirstOrDefault(a => a.AdventureName == updatedAdventure.AdventureName);
                if (existingAdventure != null)
                {
                    existingAdventure.IsComplete = updatedAdventure.IsComplete;
                    existingAdventure.PhotoCount = updatedAdventure.PhotoCount;
                    existingAdventure.LastPhotoCaptured = updatedAdventure.LastPhotoCaptured;
                    existingAdventure.LastPhotoCode = updatedAdventure.LastPhotoCode;
                    _isCacheDirty = true;
                }
            }
        }

        public List<string> GetAllAdventureNames()
        {
            lock (_cacheLock)
            {
                return _photoCache.Select(p => p.AdventureName).Distinct().ToList();
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
                    var data = new
                    {
                        Photos = _photoCache,
                        Adventures = _adventureCache
                    };

                    _logger.LogInformation("Syncing cache with JSON file.");
                    _logger.LogInformation($"cache written : \n {string.Join(",\n", _photoCache)}");

                    var json = JsonConvert.SerializeObject(data, Formatting.Indented);
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
