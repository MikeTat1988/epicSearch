using ePicSearch.Helpers;
using ePicSearch.Infrastructure.Services;
using ePicSearch.Labels;

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
            TutorialSwitch.IsToggled = _adventureManager.ShowTutorials;

            MuteLabel.Text = EnglishLabels.MuteLabel;
            StartVideoLabel.Text = EnglishLabels.StartVideoLabel;
            TutorialLabel.Text = EnglishLabels.TutorialLabel;
            ClearLogsLabel.Text = EnglishLabels.ClearLogsLabel;
        }

        private async void OnCleanLogsClicked(object sender, EventArgs e)
        {
            var logFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, "logs.txt");

            try
            {
                File.WriteAllText(logFilePath, string.Empty);
                await PopupManager.ShowNoArrowMessage(this, "Log file has been cleared!");
            }
            catch (Exception ex)
            {
                await PopupManager.ShowNoArrowMessage(this, "Failed to clear log");
            }
        }

        private void OnMuteToggled(object sender, ToggledEventArgs e)
        {

            _adventureManager.IsMuted = e.Value;
        }

        private void OnShowStartupToggled(object sender, ToggledEventArgs e)
        {
            _adventureManager.PlayStartupVideo = e.Value;
        } 
        
        private void OnTutorialToggled(object sender, ToggledEventArgs e)
        {
            _adventureManager.ShowTutorials = e.Value;
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