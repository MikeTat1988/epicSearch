using CommunityToolkit.Maui.Views;

namespace ePicSearch.Views;

public partial class ConfirmationPopup : Popup
{
    public ConfirmationPopup(string message)
    {
        InitializeComponent();
        MessageLabel.Text = message;
    }

    private void OnNoButtonClicked(object sender, EventArgs e)
    {
        Close("No");
    }

    private void OnCheckButtonClicked(object sender, EventArgs e)
    {
        Close("Yes");
    }

    private void OnDismissClicked(object sender, EventArgs e)
    {
        Close();
    }
}
