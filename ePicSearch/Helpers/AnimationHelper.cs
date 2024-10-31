
namespace ePicSearch.Helpers
{
    public static class AnimationHelper
    {
        public static async Task AnimatePress(View element)
        {
            await element.ScaleTo(0.9, 80, Easing.Linear);
            await element.ScaleTo(1.0, 80, Easing.Linear);
        }

        public static async Task AnimateLabel(VisualElement parent, string labelName)
        {
            // Locate the label associated with the button
            var label = parent?.FindByName<Label>(labelName);
            if (label != null)
            {
                await label.ScaleTo(0.9, 80, Easing.Linear);
                await label.ScaleTo(1.0, 80, Easing.Linear);
            }
        }
    }
}
