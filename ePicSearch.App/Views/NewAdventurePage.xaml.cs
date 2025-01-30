using ePicSearch.Infrastructure.Services;
using ePicSearch.Infrastructure.Entities;
using ePicSearch.Helpers;
using ePicSearch.Services;
using ePicSearch.Entities;
using ePicSearch.Labels;
using Microsoft.Extensions.Logging;

namespace ePicSearch.Views
{
    public partial class NewAdventurePage : ContentPage
    {
        private readonly AdventureManager _adventureManager;
        private readonly AdventureNameGenerator _nameGenerator;
        private readonly AudioPlayerService _audioPlayerService;
        private readonly ILogger<MainPage> _logger;

        public NewAdventurePage(AdventureManager adventureManager, AudioPlayerService audioPlayerService, AdventureNameGenerator nameGenerator, ILogger<MainPage> logger)
        {
            InitializeComponent();
            _adventureManager = adventureManager;
            _nameGenerator = nameGenerator;
            _audioPlayerService = audioPlayerService;
            _logger = logger;

            AdventureNameEntry.Text = _nameGenerator.GenerateUniqueName();

            ShowTutorialIfEnabled();
        }

        private async void ShowTutorialIfEnabled()
        {
            if (_adventureManager.ShowTutorials)
            {
                await PopupManager.ShowNoArrowMessage(this, "Welcome to a short tutorial! You can disable it anytime in the settings!", AdventureLabel);

                var messages = EnglishLabels.NewAdventurePageTutorialMessages;
                await PopupManager.ShowMessages(this, messages);
            }
        }

        private async void OnStartCreatingClicked(object sender, EventArgs e)
        {
            ClickButton(sender);

            var adventureName = await GetValidAdventureNameAsync();

            if (adventureName == null)
            {
                return;
            }

            var adventureData = new AdventureData
            {
                AdventureName = adventureName,
                IsComplete = false,
                PhotoCount = 0,
                LastPhotoCaptured = null,
                LastPhotoCode = null
            };

            _adventureManager.AddAdventure(adventureData);

            await Navigation.PushAsync(new CameraPage(adventureData, _adventureManager, _audioPlayerService, _logger));
        }

        private async Task<string?> GetValidAdventureNameAsync()
        {
            string adventureName = AdventureNameEntry.Text;

            if (string.IsNullOrEmpty(adventureName))
            {
                ErrorMessage.Text = "Enter a title";
                ErrorMessage.IsVisible = true;

                await Task.Delay(2000);
                ErrorMessage.IsVisible = false;
                return null;
            }

            if (adventureName.Length > 16)
            {
                // Show the “Too Long” message frame, then hide it
                TooLongFrame.IsVisible = true;
                await Task.Delay(1500);  // or 2000 ms
                TooLongFrame.IsVisible = false;
                return null;
            }

            var existingAdventures = _adventureManager.GetAllAdventureNames();

            if (existingAdventures.Contains(adventureName, StringComparer.OrdinalIgnoreCase))
            {
                await DisplayAlert("Oops!", "This adventure name already exists. Please choose a different name.", "OK");
                return null;
            }

            return adventureName;
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            ClickButton(sender);

            if (Navigation.NavigationStack.Count > 1)
            {
                await Navigation.PopAsync();
            }
        }

        private async void ClickButton(object sender)
        {
            await Task.WhenAll(
            AnimationHelper.AnimatePress((View)sender),
            _audioPlayerService.PlaySoundAsync(SoundLabels.ButtonPress));
        }
    }
}