using CommunityToolkit.Maui.Views;

namespace ePicSearch.Views;

public partial class TutorialPopup : Popup
{
    public TutorialPopup(string message)
    {
        InitializeComponent();
        MessageLabel.Text = message;

    }

    private void OnDismissClicked(object sender, EventArgs e)
    {
        Close();
    }
}
