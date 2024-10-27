namespace ePicSearch.Infrastructure.Entities
{
    public class AdventureData
    {
        public string AdventureName { get; set; } = "";
        public bool IsComplete { get; set; } = false;
        public int PhotoCount { get; set; } = 0;
        public string? LastPhotoCaptured { get; set; }
        public string? LastPhotoCode { get; set; }

        public override string ToString()
        {
            return $"AdventureData [AdventureName: {AdventureName}, IsComplete: {IsComplete}, " +
                   $"PhotoCount: {PhotoCount}, LastPhotoCaptured: {LastPhotoCaptured}, LastPhotoCode: {LastPhotoCode}]";
        }
    }
}
