
namespace ePicSearch.Services
{
    public class PhotoStorageService
    {
        public async Task<string> SavePhotoAsync(FileResult photo, string adventureName)
        {
            // Create a folder for the current adventure if it doesn't exist
            string adventureFolderPath = Path.Combine(FileSystem.AppDataDirectory, adventureName);

            if (!Directory.Exists(adventureFolderPath))
            {
                Directory.CreateDirectory(adventureFolderPath);
            }

            // Save the photo in the adventure-specific folder
            string localPath = Path.Combine(adventureFolderPath, photo.FileName);

            using (Stream sourceStream = await photo.OpenReadAsync())
            using (FileStream localFileStream = File.OpenWrite(localPath))
            {
                await sourceStream.CopyToAsync(localFileStream);
            }

            return localPath;
        }
    }
}
