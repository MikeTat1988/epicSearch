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

            var popup = new TutorialPopup(entry.Value)
            {
                Anchor = view
            };

            await page.ShowPopupAsync(popup);
        }
    }

    public static async Task ShowNoArrowMessage(ContentPage page, string message, View? anchor = null, double? fontSize = null, Color? fontColor = null)
    {
        var popup = new MessagePopup(message);

        if (fontSize != null)
        {
            popup.MessageLabelProperty.FontSize = fontSize.Value;
        }

        if (fontColor != null)
        {
            popup.MessageLabelProperty.TextColor = fontColor;
        }

        if (anchor != null)
        {
            popup.Anchor = anchor;
        }

        await page.ShowPopupAsync(popup);
    }

    public static async Task<bool> ShowConfirmationPopup(ContentPage page, string message, View? anchor = null)
    {
        var popup = new ConfirmationPopup(message);

        if (anchor != null)
        {
            popup.Anchor = anchor;
        }

        var result = await page.ShowPopupAsync(popup);

        return result as string == "Yes";
    }
}
        