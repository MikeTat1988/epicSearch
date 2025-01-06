using Microsoft.Maui.Storage;
using ePicSearch.Infrastructure.Entities.Interfaces;

namespace ePicSearch.Services
{
    public class FileSystemService : IFileSystemService
    {
        public string GetAppDataDirectory() => FileSystem.AppDataDirectory;

        public bool FileExists(string path) => File.Exists(path);

        public string ReadAllText(string path) => File.ReadAllText(path);

        public void WriteAllText(string path, string content) => File.WriteAllText(path, content);
    }
}