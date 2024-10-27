namespace ePicSearch.Views;

public partial class CameraPage : ContentPage
{
    private readonly string _adventureName;
    private int _photoCount;
    private string? _lastPhotoCaptured;
    private string? _lastPhotoCode;

    public CameraPage(string adventureName, int photoCount = 0, string? lastPhotoCaptured = null, string? lastPhotoCode = null)
    {
        InitializeComponent();
        _adventureName = adventureName;
        _photoCount = photoCount;
        _lastPhotoCaptured = lastPhotoCaptured;
        _lastPhotoCode = lastPhotoCode;
    }

    // Placeholder for photo capture logic, to be implemented in Checkpoint 3
    private void OnCapturePhotoClicked(object sender, EventArgs e)
    {
        DisplayAlert("Adventure Data",
                 $"Adventure Name: {_adventureName}\n" +
                 $"Photo Count: {_photoCount}\n" +
                 $"Last Photo Captured: {_lastPhotoCaptured ?? "None"}\n" +
                 $"Last Photo Code: {_lastPhotoCode ?? "None"}",
                 "OK");
    }
}