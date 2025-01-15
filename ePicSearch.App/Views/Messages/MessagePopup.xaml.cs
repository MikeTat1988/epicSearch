using CommunityToolkit.Maui.Views;

namespace ePicSearch.Views;

public partial class MessagePopup : Popup
{
    public MessagePopup(string message)
    {
        InitializeComponent();
        MessageLabel.Text = message;
    }

    public Label MessageLabelProperty => this.FindByName<Label>("MessageLabel");
}
