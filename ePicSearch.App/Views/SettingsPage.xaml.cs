using ePicSearch.Infrastructure.Services;

namespace ePicSearch.Views
{
    public partial class SettingsPage : ContentPage
    {
        private readonly AdventureManager _adventureManager;

        public SettingsPage(AdventureManager adventureManager)
        {
            InitializeComponent();
            _adventureManager = adventureManager;
            MuteSwitch.IsToggled = _adventureManager.IsMuted;
            ShowStartupSwitch.IsToggled = _adventureManager.PlayStartupVideo;
        }

        private void OnCleanLogsClicked(object sender, EventArgs e)
        {
            var logFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, "logs.txt");

            try
            {
                File.WriteAllText(logFilePath, string.Empty);
                DisplayAlert("Success", "Log file has been cleared.", "OK");
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Failed to clear log: {ex.Message}", "OK");
            }
        }

        private void OnMuteButtonClicked(object sender, EventArgs e)
        {
            MuteSwitch.IsToggled = MuteSwitch.IsToggled;
        }

        private void OnMuteToggled(object sender, ToggledEventArgs e)
        {

            _adventureManager.IsMuted = e.Value;
        }

        private void OnShowStartupButtonClicked(object sender, EventArgs e)
        {
            ShowStartupSwitch.IsToggled = ShowStartupSwitch.IsToggled;
        }

        private void OnShowStartupToggled(object sender, ToggledEventArgs e)
        {
            _adventureManager.PlayStartupVideo = e.Value;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _adventureManager.SyncCache();
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            if (Navigation.NavigationStack.Count > 1)
            {
                await Navigation.PopAsync();
            }
        }
    }
}