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
            LoadDataFromFile();
        }

        public void LoadDataFromFile()
        {
            lock (_cacheLock)
            {
                _logger.LogInformation($"Loading from JSON.");

                if (!_fileSystemService.FileExists(_jsonFilePath))
                {
                    _logger.LogWarning($"JSON file not found. Initializing with empty data store.");
                    _photoCache = new List<PhotoInfo>();
                    _adventureCache = new List<AdventureData>();
                }

                try
                {
                    var json = _fileSystemService.ReadAllText(_jsonFilePath);
                    var dataStore = JsonConvert.DeserializeObject<DataStore>(json) ?? new DataStore();

                    _photoCache = dataStore.Photos ?? new List<PhotoInfo>();
                    _adventureCache = dataStore.Adventures ?? new List<AdventureData>();

                    _logger.LogInformation($"Loaded {_photoCache.Count} photos and {_adventureCache.Count} adventures from JSON.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading data from JSON.");
                    _photoCache = new List<PhotoInfo>();
                    _adventureCache = new List<AdventureData>();
                }
            }
        }

        public void RemoveAdventure(string adventureName)
        {
            lock (_cacheLock)
            {
                _photoCache.RemoveAll(p => p.AdventureName == adventureName);

                _adventureCache.RemoveAll(a => a.AdventureName == adventureName);
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
                return _adventureCache.Select(a => a.AdventureName).Distinct().ToList();
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

                    var data = new DataStore
                    {
                        Photos = _photoCache,
                        Adventures = _adventureCache
                    };

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
