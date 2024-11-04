
namespace ePicSearch.Labels
{
    public static class TutorialTexts
    {
        public const string Step1 = "Hide your Treasure!";
        public const string Step2 = "Take a photo of the hiding spot";
        public const string Step3 = "Recieve a code and write it down";
        public const string Step4 = "Hide the code somewhere";
        public const string Step5 = "Take the photo of the hiding place";
        public const string Step6 = "Repeat!";

        public static string GetTextForStep(int stepIndex)
        {
            return stepIndex switch
            {
                0 => Step1,
                1 => Step2,
                2 => Step3,
                3 => Step4,
                4 => Step5,
                5 => Step6,
                _ => string.Empty,
            };
        }
    }
}
