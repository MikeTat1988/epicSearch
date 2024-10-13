using Xunit;
using ePicSearch.Services;
using ePicSearch.Entities;
using Microsoft.Maui.Storage;
using Moq;
using System.IO;
using System.Threading.Tasks;

namespace ePicSearch.Tests.Services
{
    public class PhotoStorageServiceTests
    {
        private readonly PhotoStorageService _photoStorageService;
        private readonly string _testDirectory;

        public PhotoStorageServiceTests()
        {
            // Use a temporary directory path for testing
            _testDirectory = Path.Combine(Path.GetTempPath(), "TestAdventure");
            Directory.CreateDirectory(_testDirectory);

            // Inject the test directory path into the service
            _photoStorageService = new PhotoStorageService(_testDirectory);
        }

        [Fact]
        public async Task SavePhotoAsync_CopiesPhotoAndDeletesOriginal()
        {
            // Arrange
            string originalPath = Path.Combine(_testDirectory, "originalPhoto.jpg");
            string newFilePath = Path.Combine(_testDirectory, "1234_1.jpg");

            await File.WriteAllTextAsync(originalPath, "Dummy photo content");

            var photoInfo = new PhotoInfo { AdventureName = "TestAdventure", Name = "1234_1" };

            // Mock FileResult using Moq
            var mockFileResult = new Mock<FileResult>(originalPath);
            mockFileResult.Setup(fr => fr.FullPath).Returns(originalPath);
            mockFileResult.Setup(fr => fr.FileName).Returns("originalPhoto.jpg");

            // Act
            string resultPath = await _photoStorageService.SavePhotoAsync(mockFileResult.Object, photoInfo);

            // Assert
            Assert.Equal(newFilePath, resultPath);
            Assert.True(File.Exists(newFilePath));
            Assert.False(File.Exists(originalPath));

            // Cleanup
            if (File.Exists(newFilePath))
            {
                File.Delete(newFilePath);
            }
        }

        [Fact]
        public void DeletePhoto_DeletesExistingPhoto()
        {
            // Arrange
            string filePath = Path.Combine(_testDirectory, "photoToDelete.jpg");
            File.WriteAllText(filePath, "Dummy photo content");

            // Act
            _photoStorageService.DeletePhoto(filePath);

            // Assert
            Assert.False(File.Exists(filePath));
        }

        [Fact]
        public void GetPhotoPath_ReturnsCorrectPath_WhenPhotoExists()
        {
            // Arrange
            string fileName = "1234_3.jpg";
            string fullPath = Path.Combine(_testDirectory, fileName);

            File.WriteAllText(fullPath, "Dummy photo content");

            // Act
            string retrievedPath = _photoStorageService.GetPhotoPath(fileName, "TestAdventure");

            // Assert
            Assert.Equal(fullPath, retrievedPath);
        }

        [Fact]
        public void GetPhotoPath_ThrowsFileNotFoundException_WhenPhotoDoesNotExist()
        {
            // Act & Assert
            Assert.Throws<FileNotFoundException>(() =>
                _photoStorageService.GetPhotoPath("nonExistent.jpg", "TestAdventure"));
        }

        public void Dispose()
        {
            // Clean up the test directory after each test
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
