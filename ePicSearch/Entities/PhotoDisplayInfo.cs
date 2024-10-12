namespace ePicSearch.Entities
{
    public class PhotoDisplayInfo
    {
        public PhotoInfo Photo { get; set; }
        public string DisplaySerialNumber { get; set; } = "";
        public bool ShouldShowCode { get; set; } = false;

        public PhotoDisplayInfo(PhotoInfo photo, int index)
        {
            Photo = photo;
            DisplaySerialNumber = (index == 0) ? "Treasure!" : (index + 1).ToString();
            ShouldShowCode = (index != 0);
        }
    }
}

