using CommunityToolkit.Maui.Views;
using ePicSearch.Views;

namespace ePicSearch.Helpers;

public static class PopupManager
{
    public static async Task ShowMessages(ContentPage page, Dictionary<string, string> messages)
    {
        foreach (var entry in messages)
        {
            if (page.FindByName<VisualElement>(entry.Key) is not View view)
                continue;

            // Create and configure the popup
            var popup = new MessagePopup(entry.Value)
            {
                Anchor = view
            };

            await page.ShowPopupAsync(popup);
        }
    }
}
        