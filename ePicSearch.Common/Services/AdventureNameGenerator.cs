namespace ePicSearch.Infrastructure.Services
{
    public class AdventureNameGenerator(AdventureManager adventureManager)
    {
        private readonly AdventureManager _adventureManager = adventureManager;
        private readonly Random _random = new();

        private readonly List<string> _adjectives = new()
        {
            "Mystic", "Sparkly", "Funny", "Giggle", "Silly",
            "Cosmic", "Incredible", "Turbo", "Magic", "Cool"
        };

        private readonly List<string> _nouns = new()
        {
            "Dungeon", "Adventure", "Search", "Travel",
            "Robots", "Galaxy", "Pirates", "Wizards", "Kittens", "Volcano"
        };

        public string GenerateUniqueName()
        {
            while (true)
            {
                var candidate = GenerateTwoWordName();
                var allNames = _adventureManager.GetAllAdventureNames();

                if (!allNames.Contains(candidate, StringComparer.OrdinalIgnoreCase))
                {
                    return candidate;
                }
            }
        }

        private string GenerateTwoWordName()
        {
            string adj = _adjectives[_random.Next(_adjectives.Count)];
            string noun = _nouns[_random.Next(_nouns.Count)];
            return $"{adj} {noun}";
        }
    }
}
