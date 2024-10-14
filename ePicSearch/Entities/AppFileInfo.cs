using ePicSearch.Infrastructure.Entities.Interfaces;

namespace ePicSearch.Entities
{
    public class AppFileResult(FileResult fileResult) : IFileResult
    {
        private readonly FileResult _fileResult = fileResult;

        public string FullPath => _fileResult.FullPath;
        public string FileName => _fileResult.FileName;
    }
}
