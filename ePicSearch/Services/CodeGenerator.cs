
namespace ePicSearch.Services
{
    public class CodeGenerator
    {
        private Random _random = new Random();

        public string GenerateCode()
        {
            return _random.Next(1000, 9999).ToString();
        }
    }
}
