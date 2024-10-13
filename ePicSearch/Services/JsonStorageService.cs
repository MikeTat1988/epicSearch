using ePicSearch.Entities;
using ePicSearch.Core.Entities; //TODO: maybe remove
using Newtonsoft.Json;

namespace ePicSearch.Services
{
    public class JsonStorageService
    {
        private readonly string JsonFilePath = Path.Combine(FileSystem.AppDataDirectory, "adventures.json");

        public List<PhotoInfo> LoadAdventuresFromJson()
        {
            if (!File.Exists(JsonFilePath)) 
            {
                return new List<PhotoInfo>();
            }

            var json = File.ReadAllText(JsonFilePath);
            return JsonConvert.DeserializeObject<List<PhotoInfo>>(json) ?? new List<PhotoInfo>();
        }

        public void SaveAdventuresToJson(List<PhotoInfo> adventures)
        {
            string directory = Path.GetDirectoryName(JsonFilePath);
            //TODO: remove
            Console.WriteLine("JSON File Path: " + JsonFilePath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonConvert.SerializeObject(adventures, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(JsonFilePath, json);
        }
    }
}
