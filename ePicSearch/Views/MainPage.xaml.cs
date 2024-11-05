using ePicSearch.Behaviors;
using ePicSearch.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using ePicSearch.Helpers;

namespace ePicSearch.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly AdventureManager _adventureManager;
        private readonly ILogger<MainPage> _logger;
        private bool _isBlurred = false;

        public MainPage(AdventureManager adventureMAnager, ILogger<MainPage> logger)
        {
            InitializeComponent();
            _adventureManager = adventureMAnager;
            _logger = logger;

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
                var fadeTask = BackgroundImage.FadeTo(0.6, 2000, Easing.CubicInOut);
                var buttonsAppearTask = ButtonsAppear(MyAdventuresButton, CreateAdventureButton, SettingsButton, QuitButton, TutorialButton, TitleImage);

                await Task.WhenAll(zoomTask, blurTask!, fadeTask, buttonsAppearTask);

                _isBlurred = true;
            }

            await CheckForIncompleteAdventure();
        }

        private async Task ButtonsAppear(params NoRippleImageButton[] buttons)
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
            await AnimationHelper.AnimatePress((View)sender);

            await Navigation.PushAsync(new SettingsPage());
        }

        private async void OnMyAdventuresClicked(object sender, EventArgs e)
        {
            await AnimationHelper.AnimatePress((View)sender);

            await Navigation.PushAsync(new MyAdventuresPage(_adventureManager, _logger));
        }
        
        private async void OnTutorialClicked(object sender, EventArgs e)
        {
            await AnimationHelper.AnimatePress((View)sender);

            await Navigation.PushAsync(new TutorialPage(_logger));
        }

        private async void OnCreateNewAdventureClicked(object sender, EventArgs e)
        {
            await AnimationHelper.AnimatePress((View)sender);

            await Navigation.PushAsync(new NewAdventurePage(_adventureManager));
        }

        private async void OnQuitClicked(object sender, EventArgs e)
        {
            await AnimationHelper.AnimatePress((View)sender);

            Application.Current?.Quit();
        }

        private async Task CheckForIncompleteAdventure()
        {
            var incompleteAdventure = _adventureManager.GetIncompleteAdventure();

            if (incompleteAdventure != null)
            {
                _logger.LogInformation("Incomplete adventure found: " + incompleteAdventure.AdventureName);
                ResumePromptModal.Initialize(incompleteAdventure, _adventureManager);

                ResumePromptModal.ModalClosed += ResumePromptModal_ModalClosed;

                ResumeAdventureModal.IsVisible = true;
            }
            else
            {
                _logger.LogInformation("No incomplete adventure found");
            }
        }

        private void ResumePromptModal_ModalClosed(object sender, EventArgs e)
        {
            // Unsubscribe from the event
            ResumePromptModal.ModalClosed -= ResumePromptModal_ModalClosed;

            ResumeAdventureModal.IsVisible = false;
        }
    }
}
