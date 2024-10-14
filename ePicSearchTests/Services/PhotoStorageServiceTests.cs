using System;
using System.IO;
using System.Threading.Tasks;
using ePicSearch.Infrastructure.Services;
using ePicSearch.Infrastructure.Entities;
using ePicSearch.Infrastructure.Entities.Interfaces;

namespace ePicSearch.Tests.Services
{
    public class PhotoStorageServiceCoreTests : IDisposable
    {
        private readonly PhotoStorageService _photoStorageService;
        private readonly string _testAppDataDirectory;

        public PhotoStorageServiceCoreTests()
        {
            // Use a temporary directory path for testing
            _testAppDataDirectory = Path.Combine(Path.GetTempPath(), "TestAppData");
            Directory.CreateDirectory(_testAppDataDirectory);

            // Mock IFileSystemService to return the test directory
            var mockFileSystemService = new TestFileSystemService(_testAppDataDirectory);

            // Initialize the service with the mocked IFileSystemService
            _photoStorageService = new PhotoStorageService(mockFileSystemService);
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

    // Test implementation of IFileSystemService
    public class TestFileSystemService : IFileSystemService
    {
        private readonly string _appDataDirectory;

        public TestFileSystemService(string appDataDirectory)
        {
            _appDataDirectory = appDataDirectory;
        }

        public string GetAppDataDirectory() => _appDataDirectory;

        public bool FileExists(string path) => File.Exists(path);

        public string ReadAllText(string path) => File.ReadAllText(path);

        public void WriteAllText(string path, string content) => File.WriteAllText(path, content);
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
