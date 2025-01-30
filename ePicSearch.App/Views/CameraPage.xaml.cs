using ePicSearch.Entities;
using ePicSearch.Helpers;
using ePicSearch.Infrastructure.Entities;
using ePicSearch.Infrastructure.Services;
using ePicSearch.Labels;
using ePicSearch.Services;
using Microsoft.Extensions.Logging;
namespace ePicSearch.Views;

public partial class CameraPage : ContentPage
{
    private AdventureData _localAdventureData;
    private readonly AdventureManager _adventureManager;
    private readonly AudioPlayerService _audioPlayerService;
    private CancellationTokenSource _cts;
    private ProgressBar? _currentLongPressProgress;
    private bool _hasShownTutorials = false;
    private readonly ILogger<MainPage> _logger;

    public CameraPage(AdventureData adventureData, AdventureManager adventureManager, AudioPlayerService audioPlayerService, ILogger<MainPage> logger)
    {
        InitializeComponent();

        _localAdventureData = new AdventureData(adventureData);
        _adventureManager = adventureManager;
        _audioPlayerService = audioPlayerService;
        _logger = logger;

        TreasureNextClueButton.Pressed += OnButtonPressed;
        TreasureNextClueButton.Released += OnButtonReleased;
        ClueNextButton.Pressed += OnButtonPressed;
        ClueNextButton.Released += OnButtonReleased;

        LongPressMessageLabel.Text = EnglishLabels.LongPressMessage;
        ClueLongPressMessageLabel.Text = EnglishLabels.LongPressMessage;

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

    protected override bool OnBackButtonPressed()
    {
        // Prevent back navigation
        return true;
    }

    private async void StartTreasurePhotoCapture()
    {
        try
        {
            var photo = await MediaPicker.CapturePhotoAsync();

            if (photo == null)
            {
                _logger.LogError($"Photo capture failed for {_localAdventureData.AdventureName}");

                var deleteSuccess = await _adventureManager.DeleteAdventureAsync(_localAdventureData.AdventureName);
                if (!deleteSuccess)
                {
                    _logger.LogWarning($"Failed to delete unfinished adventure: {_localAdventureData.AdventureName}");
                }

                await Navigation.PopAsync();
                return;
            }

            var capturedPhoto = await _adventureManager.CapturePhoto(new AppFileResult(photo), _localAdventureData.AdventureName);

            if (capturedPhoto == null)
            {
                _logger.LogError($"Failed to save the photo for {_localAdventureData.AdventureName}");
                await DisplayAlert("Error", "Failed to save the photo. Please try again.", "OK");
                return;
            }

            AddPhotoToLocalAdventure(capturedPhoto);

            TreasureCodeLabel.Text = $"{capturedPhoto.Code}";
            TreasurePhotoModal.IsVisible = true;

            if (_adventureManager.ShowTutorials)
            {
                var messages = EnglishLabels.CameraPageTreasureTutorialMessages;
                await PopupManager.ShowMessages(this, messages);
            }

            await TreasurePhotoModal.FadeTo(1, 250);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An unexpected error occurred: {ex.Message}");
            await DisplayAlert("Error", $"An unexpected error occurred", "OK");
        }
    }

    private async Task StartCluePhotoLoop()
    {
        try
        {
            var photo = await MediaPicker.CapturePhotoAsync();

            if (photo == null)
            {
                _logger.LogError($"Photo capture failed / bacvk pressed for {_localAdventureData.AdventureName}");
                await ShowResumeModal();
                return;
            }

            var capturedPhoto = await _adventureManager.CapturePhoto(new AppFileResult(photo), _localAdventureData.AdventureName);

            if (capturedPhoto == null)
            {
                _logger.LogError($"Failed to save the photo for {_localAdventureData.AdventureName}");
                await DisplayAlert("Error", "Failed to save the photo. Please try again.", "OK");
                return;
            }

            AddPhotoToLocalAdventure(capturedPhoto);

            // Use PopupManager for adventure decision
            bool continueAdventure = await PopupManager.ShowAdventureDecisionPopup(this);

            if (!continueAdventure)
            {
                await OnFinishAdventureClickedAsync(this, EventArgs.Empty);
                return;
            }

            ClueCodeLabel.Text = $"{capturedPhoto.Code}";
            CluePhotoPromptModal.IsVisible = true;
            await CluePhotoPromptModal.FadeTo(1, 250);

            if (!_hasShownTutorials)
            {
                if (_adventureManager.ShowTutorials)
                {
                    var messages = EnglishLabels.CameraPageClueTutorialMessages;
                    await PopupManager.ShowMessages(this, messages);
                }
                _hasShownTutorials = true;
            }

            SyncIfValid();
        }
        catch (Exception ex)
        {
            _logger.LogError($"An unexpected error occurred: {ex.Message}");
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
        await OnFinishAdventureClickedAsync(sender,e);
    }

    private async Task OnFinishAdventureClickedAsync(object sender, EventArgs e)
    {
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
            CompletionImage.FadeTo(1, 100),
            CompletionImage.ScaleTo(1.5, 150, Easing.CubicOut),
            flashOverlay.FadeTo(0.8, 50),
            flashOverlay.FadeTo(0, 100)
        );
        (Content as Grid)?.Children.Remove(flashOverlay);

        _audioPlayerService.PlaySoundAsync(SoundLabels.AdventureCompleted);

        await CompletionImage.ScaleTo(0.9, 100, Easing.CubicIn);
        await CompletionImage.ScaleTo(1.1, 80, Easing.BounceOut);
        await CompletionImage.ScaleTo(1, 80, Easing.BounceIn);

        CompletionTitle.IsVisible = true;
        await CompletionTitle.ScaleTo(1.5, 80, Easing.Linear);
        await CompletionTitle.ScaleTo(1.0, 80, Easing.Linear);

        await Task.Delay(400);

        await CompletionImage.FadeTo(0, 250);
        await CompletionTitle.FadeTo(0, 250);
        CompletionImage.IsVisible = false;
        CompletionTitle.IsVisible = false;

        await Shell.Current.GoToAsync("//MainPage");
    }

    private async Task ShowResumeModal()
    {
        // Save the current state of the adventure
        _adventureManager.UpdateAdventure(_localAdventureData);

        var resumePage = new ResumeAdventurePromptPage(_logger);
        resumePage.Initialize(_localAdventureData, _adventureManager, _audioPlayerService);
        resumePage.ModalClosed += (sender, e) =>
        {
            // Navigate back to the main page if the adventure is canceled
            if (!_localAdventureData.IsComplete)
            {
                Shell.Current.GoToAsync("//MainPage");
            }
        };

        await Navigation.PushModalAsync(resumePage, false); 
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

            if (_currentLongPressProgress.Progress < 1)
            {
                ShowLongPressMessage();
            }
        }
    }

    private async void ShowLongPressMessage()
    {
        LongPressMessage.IsVisible = true;
        ClueLongPressMessage.IsVisible = true;

        await Task.Delay(500); 
        
        LongPressMessage.IsVisible = false;
        ClueLongPressMessage.IsVisible = false;
    }

    private async void StartLongPressAnimation(CancellationToken token, NoRippleImageButton pressedButton)
    {
        if (_currentLongPressProgress == null)
            return;

        try
        {
            var progressTask = _currentLongPressProgress.ProgressTo(1, 1000, Easing.CubicInOut);

            // Wait for the full duration or until canceled
            await Task.WhenAny(Task.Delay(1000, token), progressTask);

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