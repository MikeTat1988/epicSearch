using ePicSearch.Entities;
using ePicSearch.Helpers;
using ePicSearch.Infrastructure.Entities;
using ePicSearch.Infrastructure.Services;
using ePicSearch.Services;
namespace ePicSearch.Views;

public partial class CameraPage : ContentPage
{
    private AdventureData _localAdventureData;
    private readonly AdventureManager _adventureManager;
    private readonly AudioPlayerService _audioPlayerService;
    private CancellationTokenSource _cts;
    private ProgressBar? _currentLongPressProgress;

    public CameraPage(AdventureData adventureData, AdventureManager adventureManager, AudioPlayerService audioPlayerService)
    {
        InitializeComponent();

        _localAdventureData = new AdventureData(adventureData);
        _adventureManager = adventureManager;
        _audioPlayerService = audioPlayerService;

        TreasureNextClueButton.Pressed += OnButtonPressed;
        TreasureNextClueButton.Released += OnButtonReleased;
        ClueNextButton.Pressed += OnButtonPressed;
        ClueNextButton.Released += OnButtonReleased;

        // Determine the starting point based on photo count
        if (_localAdventureData.PhotoCount == 0)
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

        var capturedPhoto = await _adventureManager.CapturePhoto(new AppFileResult(photo), _localAdventureData.AdventureName);

        if (capturedPhoto == null)
        {
            await DisplayAlert("Error", "Failed to save the photo. Please try again.", "OK");
            return;
        }

        AddPhotoToLocalAdventure(capturedPhoto);

        TreasureCodeLabel.Text = $"{capturedPhoto.Code}";
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

            var capturedPhoto = await _adventureManager.CapturePhoto(new AppFileResult(photo), _localAdventureData.AdventureName);

            if (capturedPhoto == null)
            {
                await DisplayAlert("Error", "Failed to save the photo. Please try again.", "OK");
                return;
            }

            AddPhotoToLocalAdventure(capturedPhoto);
            ClueCodeLabel.Text = $"{capturedPhoto.Code}";
            CluePhotoPromptModal.IsVisible = true;
            await CluePhotoPromptModal.FadeTo(1, 250);

            SyncIfValid();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }

    private void AddPhotoToLocalAdventure(PhotoInfo capturedPhoto)
    {
        _localAdventureData.LastPhotoCaptured = capturedPhoto.FilePath;
        _localAdventureData.LastPhotoCode = capturedPhoto.Code;
        _localAdventureData.PhotoCount++;
    }

    private async void OnFirstClueClicked(object sender, EventArgs e)
    {
        await TreasurePhotoModal.FadeTo(0, 250);
        TreasurePhotoModal.IsVisible = false;

        await StartCluePhotoLoop();
    }

    private async void OnNextClueClicked(object sender, EventArgs e)
    {
        await OnNextClueClickedAsync();
    }

    private async Task OnNextClueClickedAsync()
    {
        await CluePhotoPromptModal.FadeTo(0, 250);
        CluePhotoPromptModal.IsVisible = false;

        await StartCluePhotoLoop(); 
    }

    internal void SyncIfValid()
    {
        if (_localAdventureData.PhotoCount >= 2)
        {
            _adventureManager.UpdateAdventure(_localAdventureData);
        }
    }

    internal async void OnFinishAdventureClicked(object sender, EventArgs e)
    {
        await AnimationHelper.AnimatePress((View)sender);

        await CluePhotoPromptModal.FadeTo(0, 250);
        CluePhotoPromptModal.IsVisible = false;

        _localAdventureData.IsComplete = true;

        var allPhotos = _adventureManager.GetPhotosForAdventure(_localAdventureData.AdventureName);
        var lastPhoto = allPhotos[allPhotos.Count() - 1];
        lastPhoto.IsLocked = false;
        _adventureManager.UpdatePhotoState(lastPhoto);

        _adventureManager.UpdateAdventure(_localAdventureData);

        await ShowAdventureCompletion();
    }

    private async Task ShowAdventureCompletion()
    {
        CompletionImage.IsVisible = true;
        CompletionImage.Scale = 0.5; 
        CompletionImage.Opacity = 0;

        // Flash effect 
        var flashOverlay = new BoxView
        {
            BackgroundColor = Colors.White,
            Opacity = 0,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        (Content as Grid)?.Children.Add(flashOverlay);

        await Task.WhenAll(
            _audioPlayerService.PlaySoundAsync(SoundLabels.AdventureCompleted),
            CompletionImage.FadeTo(1, 100),                       // Quick fade-in
            CompletionImage.ScaleTo(1.5, 150, Easing.CubicOut),    // Strong pop to 1.5 scale
            flashOverlay.FadeTo(0.8, 50),
            flashOverlay.FadeTo(0, 100)
        );
        (Content as Grid)?.Children.Remove(flashOverlay);

        await CompletionImage.ScaleTo(0.9, 100, Easing.CubicIn);   // Slight shrink back
        await CompletionImage.ScaleTo(1.1, 80, Easing.BounceOut);  // Small bounce up
        await CompletionImage.ScaleTo(1, 80, Easing.BounceIn);     // Settle to normal scale

        await Task.Delay(200);

        // Navigate back to the main page or adventures list
        await Navigation.PopToRootAsync();

        await Task.Delay(250);
        await CompletionImage.FadeTo(0, 250);
        CompletionImage.IsVisible = false; 
    }


    private async void OnButtonPressed(object sender, EventArgs e)
    {
        _cts = new CancellationTokenSource();

        // Determine which ProgressBar to use
        if (sender == TreasureNextClueButton)
        {
            _currentLongPressProgress = TreasureLongPressProgress;
        }
        else if (sender == ClueNextButton)
        {
            _currentLongPressProgress = ClueLongPressProgress;
        }
        else
        {
            return;
        }

        _currentLongPressProgress.Progress = 0;
        _currentLongPressProgress.IsVisible = true;

        // Start the progress animation and pass the sender
        StartLongPressAnimation(_cts.Token, sender as NoRippleImageButton);
    }

    private void OnButtonReleased(object sender, EventArgs e)
    {
        // Cancel the long press if released early
        _cts?.Cancel();
        if (_currentLongPressProgress != null)
        {
            _currentLongPressProgress.IsVisible = false;
            _currentLongPressProgress.Progress = 0;
        }
    }

    private async void StartLongPressAnimation(CancellationToken token, NoRippleImageButton pressedButton)
    {
        if (_currentLongPressProgress == null)
            return;

        try
        {
            var progressTask = _currentLongPressProgress.ProgressTo(1, 1500, Easing.CubicInOut);

            // Wait for the full duration or until canceled
            await Task.WhenAny(Task.Delay(1500, token), progressTask);

            if (!token.IsCancellationRequested)
            {
                if (pressedButton == TreasureNextClueButton)
                {
                    OnFirstClueClicked(pressedButton, EventArgs.Empty);
                }
                else if (pressedButton == ClueNextButton)
                {
                    await OnNextClueClickedAsync();
                }
            }
        }
        catch (TaskCanceledException)
        {
            // Reset progress if canceled
            _currentLongPressProgress.Progress = 0;
        }
        finally
        {
            _currentLongPressProgress.IsVisible = false;
            _currentLongPressProgress = null; // Reset for next use
        }
    }
}