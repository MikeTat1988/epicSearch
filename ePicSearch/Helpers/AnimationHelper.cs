
namespace ePicSearch.Helpers
{
    public static class AnimationHelper
    {
        public static async Task AnimatePress(View element)
        {
            await element.ScaleTo(0.9, 80, Easing.Linear);
            await element.ScaleTo(1.0, 80, Easing.Linear);
        }
    }
}
