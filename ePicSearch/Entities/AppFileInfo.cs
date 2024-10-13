using ePicSearch.Core.Services;
using Microsoft.Maui.Storage;

namespace ePicSearch.Services
{
    public class AppFileResult : IFileResult
    {
        private readonly FileResult _fileResult;

        public AppFileResult(FileResult fileResult)
        {
            _fileResult = fileResult;
        }

        public string FullPath => _fileResult.FullPath;
        public string FileName => _fileResult.FileName;
    }
}
