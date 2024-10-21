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
        private Mock<IFileSystemService> _mockFileSystemService;
        private Mock<ILogger<DataStorageService>> _mockLogger;
        private DataStorageService _dataStorageService;

        [TestInitialize]
        public void Setup()
        {
            _mockFileSystemService = new Mock<IFileSystemService>();
            _mockLogger = new Mock<ILogger<DataStorageService>>();

            _mockFileSystemService.Setup(fs => fs.GetAppDataDirectory()).Returns("/mock/path");

            _dataStorageService = new DataStorageService(_mockFileSystemService.Object, _mockLogger.Object);
        }

        [TestMethod]
        public void LoadAdventuresFromFile_ShouldLoadValidAdventures()
        {
            // Arrange
            var mockJson = "[{\"FilePath\":\"/path/photo1.jpg\",\"Name\":\"photo1\",\"Code\":\"1234\",\"SerialNumber\":1,\"AdventureName\":\"test\",\"IsLocked\":true}]";
            _mockFileSystemService.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);
            _mockFileSystemService.Setup(fs => fs.ReadAllText(It.IsAny<string>())).Returns(mockJson);

            // Act
            var result = _dataStorageService.LoadAdventuresFromFile();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("photo1", result[0].Name);
        }

        [TestMethod]
        public void LoadAdventuresFromFile_ShouldReturnEmptyListIfFileDoesNotExist()
        {
            // Arrange
            _mockFileSystemService.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(false);

            // Act
            var result = _dataStorageService.LoadAdventuresFromFile();

            // Assert
            Assert.AreEqual(0, result.Count);
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
    }
}
