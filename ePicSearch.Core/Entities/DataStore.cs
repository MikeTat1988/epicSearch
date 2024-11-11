namespace ePicSearch.Infrastructure.Entities
{
    public class DataStore
    {
        public List<PhotoInfo> Photos { get; set; } = new List<PhotoInfo>();
        public List<AdventureData> Adventures { get; set; } = new List<AdventureData>();
        public Settings AppSettings { get; set; } = new Settings();
    }
}
