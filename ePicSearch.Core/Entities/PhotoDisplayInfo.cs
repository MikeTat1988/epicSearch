using ePicSearch.Infrastructure.Entities;
namespace ePicSearch.Entities
{
    public class PhotoDisplayInfo
    {
        public PhotoInfo Photo { get; set; }
        public double Rotation { get; set; }

        public PhotoDisplayInfo(PhotoInfo photo, double rotation, int index, int totalPhotos)
        {
            Photo = photo;
            Photo.IsLocked = (index != totalPhotos - 1);
            Rotation = rotation;
        }
    }
}

