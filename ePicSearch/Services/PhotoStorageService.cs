using ePicSearch.Entities;

namespace ePicSearch.Services
{
    public class PhotoStorageService
    {
        public async Task<string> SavePhotoAsync(FileResult photo, PhotoInfo photoInfo)
        {
            // Create a folder for the current adventure if it doesn't exist
            string adventureFolderPath = Path.Combine(FileSystem.AppDataDirectory, photoInfo.AdventureName);

            if (!Directory.Exists(adventureFolderPath))
            {
                Directory.CreateDirectory(adventureFolderPath);
            }

            // Save the photo in the adventure-specific folder
            string localPath = Path.Combine(adventureFolderPath, photoInfo.Name);

            using (Stream sourceStream = await photo.OpenReadAsync())
            using (FileStream localFileStream = File.OpenWrite(localPath))
            {
                await sourceStream.CopyToAsync(localFileStream);
            }

            return localPath;
        }
    }
}
