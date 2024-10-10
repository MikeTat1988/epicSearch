
namespace ePicSearch.Services
{
    public class PhotoManager(PhotoStorageService photoStorageService, CodeGenerator codeGenerator)
    {
        private readonly PhotoStorageService _photoStorageService = photoStorageService;
        private readonly CodeGenerator _codeGenerator = codeGenerator;
        private List<PhotoInfo> _photoList = new List<PhotoInfo>();
        private int _photoNumber = 1;

        public async Task<PhotoInfo> CapturePhoto(FileResult photo, string adventureName)
        {
            string filePath = await _photoStorageService.SavePhotoAsync(photo, adventureName);
            string photoCode = _codeGenerator.GenerateCode();
            string photoName = $"{photoCode}_{_photoNumber }";

            var photoInfo = new PhotoInfo
            {
                FilePath = filePath,
                Name = photoName,
                Code = photoCode,
                SerialNumber = _photoNumber 
            };
            _photoList.Add(photoInfo);

            _photoNumber++;
            return photoInfo;
        }

        public List<PhotoInfo> GetPhotos() => _photoList;
    }

    public class PhotoInfo
    {
        public string FilePath { get; set; } = "";
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public int SerialNumber { get; set; }
    }
}
