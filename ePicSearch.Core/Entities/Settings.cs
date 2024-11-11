namespace ePicSearch.Infrastructure.Entities
{
    public class Settings
    {
        public bool EnableNotifications { get; set; } = true;
        public int MaxPhotoCount { get; set; } = 100;

        public bool CrashFlag { get; set; } = false;
    }
}