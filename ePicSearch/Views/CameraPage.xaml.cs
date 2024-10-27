using ePicSearch.Infrastructure.Entities;
using ePicSearch.Infrastructure.Services;
namespace ePicSearch.Views;

public partial class CameraPage : ContentPage
{
    private AdventureData _adventureData;
    private readonly AdventureManager _adventureManager;

    public CameraPage(AdventureData adventureData, AdventureManager adventureManager)
    {
        InitializeComponent();

        _adventureData = adventureData;
        _adventureManager = adventureManager;

        // Determine the starting point based on photo count
        if (_adventureData.PhotoCount == 0)
        {
            StartTreasurePhotoCapture();
        }
        else
        {
            StartCluePhotoLoop();
        }
    }

    private void StartTreasurePhotoCapture()
    {
        string simulatedPhotoPath = "/path/to/treasure_photo.jpg";
        string generatedCode = _adventureManager.GenerateCode();  // Generate a unique code

        // Update adventure data
        _adventureData.LastPhotoCaptured = simulatedPhotoPath;
        _adventureData.LastPhotoCode = generatedCode;
        _adventureData.PhotoCount = 1;  // This is the first photo in the adventure

        // Display the unique code in the TreasureCodeLabel
        TreasureCodeLabel.Text = $"Code: {generatedCode}";

        // Make the TreasurePhotoModal visible
        TreasurePhotoModal.IsVisible = true;

        // Update the adventure data in storage
        _adventureManager.UpdateAdventure(_adventureData);
    }

    private void OnNextClueClicked(object sender, EventArgs e)
    {
        // Hide the TreasurePhotoModal
        TreasurePhotoModal.IsVisible = false;

        // Proceed to the clue photo loop
        StartCluePhotoLoop();
    }

    private void StartCluePhotoLoop()
    {
        // Placeholder for starting the clue photo loop

        // This will be implemented in Checkpoint 3.3

        DisplayAlert("Testing StartCluePhotoLoop reached",
                 _adventureData.ToString(),
                 "OK");
    }

    // Placeholder for photo capture logic, to be implemented in Checkpoint 3
    private void OnCapturePhotoClicked(object sender, EventArgs e)
    {
        DisplayAlert("Adventure Data",
                 _adventureData.ToString(),
                 "OK");
    }
}