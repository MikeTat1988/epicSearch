using CommunityToolkit.Maui.Views;

namespace ePicSearch.Views;

public partial class MessagePopup : Popup
{
    public MessagePopup(string message)
    {
        InitializeComponent();
        MessageLabel.Text = message;

    }

    private void OnDismissClicked(object sender, EventArgs e)
    {
        Close();
    }
}
