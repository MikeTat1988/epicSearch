using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ePicSearch.Infrastructure.Services;
using ePicSearch.Infrastructure.Entities;
using ePicSearch.Infrastructure.Entities.Interfaces;
using Microsoft.Extensions.Logging;

namespace ePicSearch.Tests
{
    [TestClass]
    public class DataStorageServiceTests
    {
        private Mock<IFileSystemService> _mockFileSystemService = new Mock<IFileSystemService>();
        private Mock<ILogger<DataStorageService>> _mockLogger = new Mock<ILogger<DataStorageService>>();
        private DataStorageService _dataStorageService;

        [TestInitialize]
        public void Setup()
        {
            _mockFileSystemService.Setup(fs => fs.GetAppDataDirectory()).Returns("/mock/path");

            _dataStorageService = new DataStorageService(_mockFileSystemService.Object, _mockLogger.Object);
        }

        [TestMethod]
        public void LoadAdventuresFromFile_ShouldLoadValidAdventures()
        {
            // Arrange
            var mockJson = "{ \"Photos\": [{ \"FilePath\": \"/path/photo1.jpg\", \"Name\": \"photo1\", \"Code\": \"1234\", \"SerialNumber\": 1, \"AdventureName\": \"test\", \"IsLocked\": true }],"
             + " \"Adventures\": [{ \"AdventureName\": \"testAdventure\", \"IsComplete\": false, \"PhotoCount\": 1, \"LastPhotoCaptured\": \"/path/photo1.jpg\", \"LastPhotoCode\": \"1234\" }] }";

            _mockFileSystemService.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);
            _mockFileSystemService.Setup(fs => fs.ReadAllText(It.IsAny<string>())).Returns(mockJson);

            // Act
            _dataStorageService.LoadDataFromFile();
            var photos = _dataStorageService.GetPhotosForAdventure("test");
            var adventures = _dataStorageService.GetAllAdventureNames();

            // Assert
            Assert.AreEqual(1, photos.Count, "Expected 1 photo to be loaded.");
            Assert.AreEqual("photo1", photos[0].Name, "Photo name does not match.");

            Assert.AreEqual(1, adventures.Count, "Expected 1 adventure to be loaded.");
            Assert.AreEqual("testAdventure", adventures[0], "Adventure name does not match.");
        }

        [TestMethod]
        public void LoadAdventuresFromFile_ShouldReturnEmptyListIfFileDoesNotExist()
        {
            // Arrange
            _mockFileSystemService.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(false);

            // Act
            _dataStorageService.LoadDataFromFile();
            var photos = _dataStorageService.GetPhotosForAdventure("test");
            var adventures = _dataStorageService.GetAllAdventureNames();

            // Assert: Verify both photos and adventures are empty
            Assert.AreEqual(0, photos.Count, "Expected 0 photos when file does not exist.");
            Assert.AreEqual(0, adventures.Count, "Expected 0 adventures when file does not exist.");
        }

        [TestMethod]
        public void AddPhoto_ShouldAddPhotoAndMarkCacheAsDirty()
        {
            // Arrange
            var photo = new PhotoInfo
            {
                FilePath = "/path/photo1.jpg",
                Name = "photo1",
                Code = "1234",
                SerialNumber = 1,
                AdventureName = "test",
                IsLocked = true
            };

            // Act
            _dataStorageService.AddPhoto(photo);
            var photos = _dataStorageService.GetPhotosForAdventure("test");

            // Assert
            Assert.AreEqual(1, photos.Count);
            Assert.IsTrue(photos[0].IsLocked);
        }

        [TestMethod]
        public void UpdatePhoto_ShouldUpdatePhotoAndMarkCacheAsDirty()
        {
            // Arrange
            var photo = new PhotoInfo
            {
                FilePath = "/path/photo1.jpg",
                Name = "photo1",
                Code = "1234",
                SerialNumber = 1,
                AdventureName = "test",
                IsLocked = true
            };
            _dataStorageService.AddPhoto(photo);

            // Act
            photo.IsLocked = false;
            _dataStorageService.UpdatePhoto(photo);
            var updatedPhoto = _dataStorageService.GetPhotosForAdventure("test")[0];

            // Assert
            Assert.IsFalse(updatedPhoto.IsLocked);
        }

        [TestMethod]
        public void RemoveAdventure_ShouldRemoveAllPhotosForAdventure()
        {
            // Arrange
            var photo1 = new PhotoInfo { AdventureName = "test", Name = "photo1" };
            var photo2 = new PhotoInfo { AdventureName = "test", Name = "photo2" };
            _dataStorageService.AddPhoto(photo1);
            _dataStorageService.AddPhoto(photo2);

            // Act
            _dataStorageService.RemoveAdventure("test");
            var photos = _dataStorageService.GetPhotosForAdventure("test");

            // Assert
            Assert.AreEqual(0, photos.Count);
        }

        [TestMethod]
        public void GetAllAdventureNames_ShouldReturnUniqueAdventureNames()
        {
            // Arrange
            _dataStorageService.AddPhoto(new PhotoInfo { AdventureName = "adventure1" });
            _dataStorageService.AddPhoto(new PhotoInfo { AdventureName = "adventure2" });
            _dataStorageService.AddPhoto(new PhotoInfo { AdventureName = "adventure1" });

            // Act
            var adventures = _dataStorageService.GetAllAdventureNames();

            // Assert
            Assert.AreEqual(2, adventures.Count);
        }

        [TestMethod]
        public void SyncCacheToFile_ShouldWriteToFileIfCacheIsDirty()
        {
            // Arrange
            var photo = new PhotoInfo { AdventureName = "test", Name = "photo1" };
            _dataStorageService.AddPhoto(photo);
            _mockFileSystemService.Setup(fs => fs.WriteAllText(It.IsAny<string>(), It.IsAny<string>()));

            // Act
            _dataStorageService.SyncCacheToFile();

            // Assert
            _mockFileSystemService.Verify(fs => fs.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void SyncCacheToFile_ShouldNotWriteIfCacheIsClean()
        {
            // Act
            _dataStorageService.SyncCacheToFile();

            // Assert
            _mockFileSystemService.Verify(fs => fs.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void AddAdventure_ShouldAddAdventureAndRetrieveIt()
        {
            // Arrange
            var adventure = new AdventureData
            {
                AdventureName = "testAdventure",
                IsComplete = false,
                PhotoCount = 3,
                LastPhotoCaptured = "/path/photo3.jpg",
                LastPhotoCode = "5678"
            };

            // Act
            _dataStorageService.AddAdventure(adventure);
            var retrievedAdventure = _dataStorageService.GetAdventureData("testAdventure");

            // Assert
            Assert.IsNotNull(retrievedAdventure);
            Assert.AreEqual("testAdventure", retrievedAdventure.AdventureName);
            Assert.AreEqual(3, retrievedAdventure.PhotoCount);
            Assert.AreEqual("/path/photo3.jpg", retrievedAdventure.LastPhotoCaptured);
            Assert.AreEqual("5678", retrievedAdventure.LastPhotoCode);
        }

        [TestMethod]
        public void UpdateAdventureData_ShouldUpdateExistingAdventure()
        {
            // Arrange
            var adventure = new AdventureData
            {
                AdventureName = "testAdventure",
                IsComplete = false,
                PhotoCount = 2,
                LastPhotoCaptured = "/path/photo2.jpg",
                LastPhotoCode = "1234"
            };
            _dataStorageService.AddAdventure(adventure);

            // Act
            var updatedAdventure = new AdventureData
            {
                AdventureName = "testAdventure",
                IsComplete = true,
                PhotoCount = 4,
                LastPhotoCaptured = "/path/photo4.jpg",
                LastPhotoCode = "4321"
            };
            _dataStorageService.UpdateAdventureData(updatedAdventure);
            var retrievedAdventure = _dataStorageService.GetAdventureData("testAdventure");

            // Assert
            Assert.IsNotNull(retrievedAdventure);
            Assert.IsTrue(retrievedAdventure.IsComplete);
            Assert.AreEqual(4, retrievedAdventure.PhotoCount);
            Assert.AreEqual("/path/photo4.jpg", retrievedAdventure.LastPhotoCaptured);
            Assert.AreEqual("4321", retrievedAdventure.LastPhotoCode);
        }

    }
}
