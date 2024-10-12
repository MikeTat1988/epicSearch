using ePicSearch.Entities;

namespace ePicSearch.Services
{
    public class PhotoStorageService
    {
        public async Task<string> SavePhotoAsync(FileResult photo, PhotoInfo photoInfo)
        {
            string adventureFolderPath = Path.Combine(FileSystem.AppDataDirectory, photoInfo.AdventureName);

            // Create a folder for the current adventure if it doesn't exist
            if (!Directory.Exists(adventureFolderPath))
            {
                Directory.CreateDirectory(adventureFolderPath);
            }

            // Append the correct file extension
            string fileExtension = Path.GetExtension(photo.FileName);
            string localPath = Path.Combine(adventureFolderPath, $"{photoInfo.Name}{fileExtension}");

            using (Stream sourceStream = await photo.OpenReadAsync())
            using (FileStream localFileStream = File.OpenWrite(localPath))
            {
                await sourceStream.CopyToAsync(localFileStream);
            }

            return localPath;
        }
    }
}
