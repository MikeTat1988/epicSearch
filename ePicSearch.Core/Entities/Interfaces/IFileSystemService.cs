namespace ePicSearch.Infrastructure.Entities.Interfaces
{
    public interface IFileSystemService
    {
        string GetAppDataDirectory();
        bool FileExists(string path);
        string ReadAllText(string path);
        void WriteAllText(string path, string content);
    }
}