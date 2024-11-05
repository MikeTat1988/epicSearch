using ePicSearch.Helpers;
using ePicSearch.Infrastructure.Entities;
using ePicSearch.Infrastructure.Services;
using System.Threading;

namespace ePicSearch.Views
{
    public partial class ResumeAdventurePromptModal : ContentView
    {
        public event EventHandler ModalClosed;

        private AdventureData _adventureData;
        private AdventureManager _adventureManager;
        private CancellationTokenSource _cts;

        public ResumeAdventurePromptModal()
        {
            InitializeComponent();
        }

        public void Initialize(AdventureData adventureData, AdventureManager adventureManager)
        {
            _adventureData = adventureData;
            _adventureManager = adventureManager;
            AdventureInfoLabel.Text = $"Photos Taken: {adventureData.PhotoCount}, Last Code: {adventureData.LastPhotoCode}";
        }

        private async void OnContinueClicked(object sender, EventArgs e)
        {
            await AnimationHelper.AnimatePress((View)sender);

            ModalClosed?.Invoke(this, EventArgs.Empty);

            // Close the modal and navigate to CameraPage to continue adventure
            this.IsVisible = false;
            await Application.Current.MainPage.Navigation.PushAsync(new CameraPage(_adventureData, _adventureManager));
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
        }

        private async void StartLongPressAnimation(CancellationToken token)
        {
            try
            {
                var progressTask = ExitLongPressProgress.ProgressTo(1, 1500, Easing.CubicInOut);
                await Task.WhenAny(Task.Delay(1500, token), progressTask);

                if (!token.IsCancellationRequested)
                {
                    // Delete the adventure and hide the modal
                    await _adventureManager.DeleteAdventureAsync(_adventureData.AdventureName);
                    ModalClosed?.Invoke(this, EventArgs.Empty);
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
    }
}
