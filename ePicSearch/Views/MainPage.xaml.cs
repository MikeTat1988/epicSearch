using ePicSearch.Behaviors;
using ePicSearch.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace ePicSearch.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly PhotoManager _photoManager;
        private readonly ILogger<MainPage> _logger;
        private readonly ILogger<GamePage> _gamePageLogger;
        private bool _isBlurred = false;

        public MainPage(PhotoManager photoManager, ILogger<MainPage> logger, ILogger<GamePage> gamePageLogger)
        {
            InitializeComponent();
            _photoManager = photoManager;
            _logger = logger;
            _gamePageLogger = gamePageLogger;

            Appearing += MainPage_Appearing;
            _logger.LogInformation("MainPage initialized.");
        }

        private async void MainPage_Appearing(object? sender, EventArgs e)
        {
            if (!_isBlurred)
            {
                // Display the background for 1 second
                await Task.Delay(1000);

                var zoomTask = BackgroundImage.ScaleTo(1.1, 2000, Easing.CubicInOut);

                var blurBehavior = BackgroundImage.Behaviors.OfType<BlurBehavior>().FirstOrDefault();
                var blurTask = blurBehavior?.AnimateBlurEffect(10, 100);
                var fadeTask = BackgroundImage.FadeTo(0.7, 2000, Easing.CubicInOut);
                var buttonsAppearTask = ButtonsAppear(MyAdventuresButton, CreateAdventureButton, SettingsButton, QuitButton);


                await Task.WhenAll(zoomTask, blurTask!, fadeTask, buttonsAppearTask);

                _isBlurred = true;
            }
        }

        private async Task ButtonsAppear(params ImageButton[] buttons)
        {
            var animationTasks = new List<Task>();

            foreach (var button in buttons)
            {
                var buttonTask = Task.WhenAll(
                    button.FadeTo(1, 1000),
                    button.TranslateTo(0, -10, 1000, Easing.CubicInOut)
                );

                animationTasks.Add(buttonTask);
            }

            await Task.WhenAll(animationTasks);
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        private async void OnMyAdventuresClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyAdventuresPage(_photoManager, _logger, _gamePageLogger));
        }

        private async void OnCreateNewAdventureClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewAdventurePage(_photoManager));
        }

        private void OnQuitClicked(object sender, EventArgs e)
        {
            Application.Current?.Quit();
        }
    }
}
