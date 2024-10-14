using ePicSearch.Infrastructure.Services;

namespace ePicSearch.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly PhotoManager _photoManager;

        // Use Dependency Injection to provide PhotoManager instance
        public MainPage(PhotoManager photoManager)
        {
            InitializeComponent();
            _photoManager = photoManager;
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        private async void OnMyAdventuresClicked(object sender, EventArgs e)
        {
            // Pass the PhotoManager instance when navigating to MyAdventuresPage
            await Navigation.PushAsync(new MyAdventuresPage(_photoManager));
        }

        private async void OnCreateNewAdventureClicked(object sender, EventArgs e)
        {
            // Pass the PhotoManager instance when navigating to NewAdventurePage
            await Navigation.PushAsync(new NewAdventurePage(_photoManager));
        }

        private void OnQuitClicked(object sender, EventArgs e)
        {
            Application.Current.Quit();
        }
    }
}
