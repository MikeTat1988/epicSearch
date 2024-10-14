namespace ePicSearch.Infrastructure.Services
{
    public interface IFileResult
    {
        string FullPath { get; }
        string FileName { get; }
    }
}