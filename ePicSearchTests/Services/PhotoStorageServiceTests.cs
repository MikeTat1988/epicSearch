using System;
using System.IO;
using System.Threading.Tasks;
using ePicSearch.Core.Entities;
using ePicSearch.Core.Services;
using Xunit;

namespace ePicSearch.Tests.Services
{
    public class PhotoStorageServiceCoreTests : IDisposable
    {
        private readonly PhotoStorageServiceCore _photoStorageService;
        private readonly string _testAppDataDirectory;

        public PhotoStorageServiceCoreTests()
        {
            // Use a temporary directory path for testing
            _testAppDataDirectory = Path.Combine(Path.GetTempPath(), "TestAppData");
            Directory.CreateDirectory(_testAppDataDirectory);

            // Initialize the service with the test directory
            _photoStorageService = new PhotoStorageServiceCore(_testAppDataDirectory);
        }

        [Fact]
        public async Task SavePhotoAsync_CopiesPhotoAndDeletesOriginal()
        {
            // Arrange
            string originalPhotoContent = "Dummy photo content";
            string originalFileName = "originalPhoto.jpg";
            string adventureName = "TestAdventure";

            // Create a temporary directory to simulate the original photo location
            string originalPhotoDirectory = Path.Combine(_testAppDataDirectory, "OriginalPhotos");
            Directory.CreateDirectory(originalPhotoDirectory);

            // Create the original photo file
            string originalPhotoPath = Path.Combine(originalPhotoDirectory, originalFileName);
            await File.WriteAllTextAsync(originalPhotoPath, originalPhotoContent);

            // Create PhotoInfo
            var photoInfo = new PhotoInfo
            {
                AdventureName = adventureName,
                Name = "1234_1",
                Code = "1234",
                SerialNumber = 1
            };

            // Create TestFileResult
            var fileResult = new TestFileResult(originalPhotoPath, originalFileName);

            // Expected new file path
            string expectedAdventureFolderPath = Path.Combine(_testAppDataDirectory, adventureName);
            string expectedNewFileName = $"{photoInfo.Name}{Path.GetExtension(originalFileName)}";
            string expectedNewFilePath = Path.Combine(expectedAdventureFolderPath, expectedNewFileName);

            // Act
            string resultPath = await _photoStorageService.SavePhotoAsync(fileResult, photoInfo);

            // Assert
            Assert.Equal(expectedNewFilePath, resultPath);
            Assert.True(File.Exists(expectedNewFilePath), "New photo file should exist.");
            Assert.False(File.Exists(originalPhotoPath), "Original photo file should have been deleted.");

            // Verify content is the same
            string newPhotoContent = await File.ReadAllTextAsync(expectedNewFilePath);
            Assert.Equal(originalPhotoContent, newPhotoContent);
        }

        public void Dispose()
        {
            // Clean up the test directories after each test
            if (Directory.Exists(_testAppDataDirectory))
            {
                Directory.Delete(_testAppDataDirectory, true);
            }
        }
    }

    // Test implementation of IFileResult
    public class TestFileResult : IFileResult
    {
        public string FullPath { get; }
        public string FileName { get; }

        public TestFileResult(string fullPath, string fileName)
        {
            FullPath = fullPath;
            FileName = fileName;
        }
    }
}
