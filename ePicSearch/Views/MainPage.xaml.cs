using ePicSearch.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace ePicSearch.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly PhotoManager _photoManager;
        private readonly ILogger<MainPage> _logger;

        public MainPage(PhotoManager photoManager, ILogger<MainPage> logger)
        {
            InitializeComponent();
            _photoManager = photoManager;
            _logger = logger;

            _logger.LogInformation("MainPage initialized.");
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        private async void OnMyAdventuresClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyAdventuresPage(_photoManager));
        }

        private async void OnCreateNewAdventureClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewAdventurePage(_photoManager));
        }

        private void OnQuitClicked(object sender, EventArgs e)
        {
            Application.Current.Quit();
        }
    }
}
