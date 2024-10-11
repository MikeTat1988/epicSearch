using ePicSearch.Entities;

namespace ePicSearch.Services
{
    public class PhotoManager(PhotoStorageService photoStorageService, CodeGenerator codeGenerator)
    {
        private readonly PhotoStorageService _photoStorageService = photoStorageService;
        private readonly CodeGenerator _codeGenerator = codeGenerator;
        private List<PhotoInfo> _photoList = new List<PhotoInfo>();

        public async Task<PhotoInfo> CapturePhoto(FileResult photo, string adventureName)
        {
            string photoCode = _codeGenerator.GenerateCode();
            int serialNumber = _photoList.Count + 1;

            var photoInfo = new PhotoInfo
            {
                FilePath = photo.FullPath,  // FullPath for now to get the actual file path
                Name = $"{photoCode}_{serialNumber}",
                Code = photoCode,
                SerialNumber = serialNumber,
                AdventureName = adventureName
            };

            // Save the photo to to the corresponding adventure folder
            photoInfo.FilePath = await _photoStorageService.SavePhotoAsync(photo, photoInfo);

            // Delete the original photo after it's been saved to the adventure folder
            if (File.Exists(photo.FullPath))
            {
                File.Delete(photo.FullPath);
            }

            _photoList.Add(photoInfo);

            return photoInfo;
        }

        public List<PhotoInfo> GetPhotosForAdventure(string adventureName)
        {
            //TODO: In a real implementation, this would retrieve previously saved photo data
            return _photoList.Where(p => p.AdventureName == adventureName).ToList();
        }
    }
}
