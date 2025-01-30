using ePicSearch.Helpers;
using ePicSearch.Infrastructure.Entities;
using ePicSearch.Infrastructure.Services;
using ePicSearch.Labels;
using ePicSearch.Services;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace ePicSearch.Views
{
    public partial class ResumeAdventurePromptPage : ContentPage
    {
        public event EventHandler ModalClosed;

        private AdventureData _adventureData;
        private AdventureManager _adventureManager;
        private AudioPlayerService _audioPlayerService;
        private CancellationTokenSource _cts;
        private readonly ILogger<MainPage> _logger;

        public ResumeAdventurePromptPage()
        {
            InitializeComponent();

            LongPressMessageLabel.Text = EnglishLabels.LongPressMessage;
        }

        public ResumeAdventurePromptPage(ILogger<MainPage> logger)
        {
            InitializeComponent();

            _logger = logger;           

            LongPressMessageLabel.Text = EnglishLabels.LongPressMessage;
        }

        public void Initialize(AdventureData adventureData, AdventureManager adventureManager, AudioPlayerService audioPlayerService)
        {
            _adventureData = adventureData;
            _adventureManager = adventureManager;
            _audioPlayerService = audioPlayerService;
            AdventureInfoLabel.Text = $"Photos Taken: {adventureData.PhotoCount}, Last Code: {adventureData.LastPhotoCode}";
        }

        private async void OnContinueClicked(object sender, EventArgs e)
        {
            await AnimationHelper.AnimatePress((View)sender);

            ModalClosed?.Invoke(this, EventArgs.Empty);

            // Pop *this* page from the modal stack
            await Application.Current.MainPage.Navigation.PopModalAsync(false);

            // Then push your CameraPage
            await Application.Current.MainPage.Navigation
                       .PushAsync(new CameraPage(_adventureData, _adventureManager, _audioPlayerService, _logger), false);
        }

        private async void OnExitButtonPressed(object sender, EventArgs e)
        {
            _cts = new CancellationTokenSource();
            ExitLongPressProgress.IsVisible = true;
            ExitLongPressProgress.Progress = 0;

            StartLongPressAnimation(_cts.Token);
        }

        private void OnExitButtonReleased(object sender, EventArgs e)
        {
            // Cancel long press if button is released early
            _cts?.Cancel();
            ExitLongPressProgress.IsVisible = false;
            ExitLongPressProgress.Progress = 0;

            ShowLongPressMessage();
        }

        private async void StartLongPressAnimation(CancellationToken token)
        {
            try
            {
                var progressTask = ExitLongPressProgress.ProgressTo(1, 1000, Easing.CubicInOut);
                await Task.WhenAny(Task.Delay(1000, token), progressTask);

                if (!token.IsCancellationRequested)
                {
                    // Delete the adventure and hide the modal
                    await _adventureManager.DeleteAdventureAsync(_adventureData.AdventureName);
                    ModalClosed?.Invoke(this, EventArgs.Empty);

                    var shell = MauiProgram.AppInstance.Services.GetRequiredService<AppShell>();
                    Application.Current.MainPage = shell;

                    await Shell.Current.GoToAsync("//MainPage");
                }
            }
            catch (TaskCanceledException)
            {
                ExitLongPressProgress.Progress = 0;
            }
            finally
            {
                ExitLongPressProgress.IsVisible = false;
            }
        }

        private async void ShowLongPressMessage()
        {
            LongPressMessage.IsVisible = true;
            await Task.Delay(500); // Show for half a second
            LongPressMessage.IsVisible = false;
        }
    }
}
