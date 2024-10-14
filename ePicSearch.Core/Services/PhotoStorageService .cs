using ePicSearch.Infrastructure.Entities;
using ePicSearch.Infrastructure.Entities.Interfaces;

namespace ePicSearch.Infrastructure.Services
{
    public class PhotoStorageService(IFileSystemService fileSystemService)
    {
        private readonly string _appDataDirectory = fileSystemService.GetAppDataDirectory();

        public async Task<string> SavePhotoAsync(IFileResult photo, PhotoInfo photoInfo)
        {
            string adventureFolderPath = Path.Combine(_appDataDirectory, photoInfo.AdventureName);

            if (!Directory.Exists(adventureFolderPath))
            {
                Directory.CreateDirectory(adventureFolderPath);
            }

            // Append the correct file extension
            string fileExtension = Path.GetExtension(photo.FileName);
            string newFilePath = Path.Combine(adventureFolderPath, $"{photoInfo.Name}{fileExtension}");

            // Copy the photo from the original path to the adventure filder
            try
            {
                using var sourceStream = File.OpenRead(photo.FullPath);
                using var destinationStream = File.Create(newFilePath);

                await sourceStream.CopyToAsync(destinationStream);

                DeletePhoto(photo.FullPath);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to save photo: {ex.Message}", ex);
            }

            return newFilePath;
        }

        public static bool DeletePhoto(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    return true;
                }
                catch (Exception ex)
                {
                    throw new IOException($"Error deleting file at {filePath}: {ex.Message}", ex);
                }
            }

            return false;
        }

        public string GetPhotoPath(string fileName, string adventureName)
        {
            string adventureFolderPath = Path.Combine(_appDataDirectory, adventureName);
            string fullPath = Path.Combine(adventureFolderPath, fileName);

            if (File.Exists(fullPath))
            {
                return fullPath;
            }
            else
            {
                throw new FileNotFoundException($"Photo file not found: {fullPath}");
            }
        }
    }
}
