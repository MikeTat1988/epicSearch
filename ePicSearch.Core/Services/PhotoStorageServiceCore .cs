using ePicSearch.Infrastructure.Entities;

namespace ePicSearch.Infrastructure.Services
{
    public class PhotoStorageServiceCore
    {
        private readonly string _appDataDirectory;

        public PhotoStorageServiceCore(string appDataDirectory)
        {
            _appDataDirectory = appDataDirectory;
        }

        public async Task<string> SavePhotoAsync(IFileResult photo, PhotoInfo photoInfo)
        {
            string adventureFolderPath = Path.Combine(_appDataDirectory, photoInfo.AdventureName);

            if (!Directory.Exists(adventureFolderPath))
            {
                Directory.CreateDirectory(adventureFolderPath);
            }

            // Append the correct file extension
            string fileExtension = Path.GetExtension(photo.FileName);
            string newFileName = $"{photoInfo.Name}{fileExtension}";
            string newFilePath = Path.Combine(adventureFolderPath, newFileName);

            // Copy the photo from the original path to the new path
            try
            {
                File.Copy(photo.FullPath, newFilePath, overwrite: true);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to copy photo from {photo.FullPath} to {newFilePath}: {ex.Message}", ex);
            }

            // Delete the original photo to prevent duplication
            try
            {
                DeletePhoto(photo.FullPath);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to delete original photo at {photo.FullPath}: {ex.Message}", ex);
            }

            return newFilePath;
        }

        public static void DeletePhoto(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    throw new IOException($"Error deleting file at {filePath}: {ex.Message}", ex);
                }
            }
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
