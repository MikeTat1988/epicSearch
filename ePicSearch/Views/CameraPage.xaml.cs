using ePicSearch.Entities;
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

    private async void StartTreasurePhotoCapture()
    {
        var photo = await MediaPicker.CapturePhotoAsync();

        if (photo == null)
        {
            await DisplayAlert("Error", "Photo capture failed. Please try again.", "OK");
            return;
        }

        var capturedPhoto = await _adventureManager.CapturePhoto(new AppFileResult(photo), _adventureData.AdventureName);

        if (capturedPhoto == null)
        {
            await DisplayAlert("Error", "Failed to save the photo. Please try again.", "OK");
            return;
        }

        AddPhotoToAdventure(capturedPhoto);

        TreasureCodeLabel.Text = $"Code: {capturedPhoto.Code}";
        TreasurePhotoModal.IsVisible = true;
        await TreasurePhotoModal.FadeTo(1, 250);
    }

    private async Task StartCluePhotoLoop()
    {
        try
        {
            var photo = await MediaPicker.CapturePhotoAsync();

            if (photo == null)
            {
                await DisplayAlert("Error", "Photo capture failed. Please try again.", "OK");
                return;
            }

            var capturedPhoto = await _adventureManager.CapturePhoto(new AppFileResult(photo), _adventureData.AdventureName);

            if (capturedPhoto == null)
            {
                await DisplayAlert("Error", "Failed to save the photo. Please try again.", "OK");
                return;
            }

            AddPhotoToAdventure(capturedPhoto);
            ClueCodeLabel.Text = $"Code: {capturedPhoto.Code}";
            CluePhotoPromptModal.IsVisible = true;
            await CluePhotoPromptModal.FadeTo(1, 250);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }

    private void AddPhotoToAdventure(PhotoInfo capturedPhoto)
    {
        _adventureData.LastPhotoCaptured = capturedPhoto.FilePath;
        _adventureData.LastPhotoCode = capturedPhoto.Code;
        _adventureData.PhotoCount++;
        _adventureManager.UpdateAdventure(_adventureData);
    }

    private async void OnFirstClueClicked(object sender, EventArgs e)
    {
        await AnimateLongPress(TreasureNextClueButton);
        await TreasurePhotoModal.FadeTo(0, 250);
        TreasurePhotoModal.IsVisible = false;

        await StartCluePhotoLoop();
    }

    private async void OnNextClueClicked(object sender, EventArgs e)
    {
        await AnimateLongPress(TreasureNextClueButton);
        await CluePhotoPromptModal.FadeTo(0, 250);
        CluePhotoPromptModal.IsVisible = false;

        await StartCluePhotoLoop(); 
    }

    private async void OnFinishAdventureClicked(object sender, EventArgs e)
    {
        await CluePhotoPromptModal.FadeTo(0, 250);
        CluePhotoPromptModal.IsVisible = false;

        _adventureData.IsComplete = true;
        _adventureManager.UpdateAdventure(_adventureData);

        await ShowAdventureCompletion();

        // Navigate back to the main page or adventures list
        await Navigation.PopToRootAsync();
    }

    private async Task AnimateLongPress(Button button)
    {
        button.Opacity = 1;
        for (double i = 0.5; i <= 1; i += 0.05)
        {
            button.Opacity = i;
            await Task.Delay(50);
        }
    }

    private async Task ShowAdventureCompletion()
    {
        var completionImage = new Image
        {
            Source = "adventure_finished.png",
            Opacity = 0,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        Content = new Grid { Children = { completionImage } };
        await completionImage.FadeTo(1, 250);
        await Task.Delay(1000);
        await completionImage.FadeTo(0, 250);
    }
}