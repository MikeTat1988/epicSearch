using ePicSearch.Infrastructure.Services;
using ePicSearch.Infrastructure.Entities;
using ePicSearch.Helpers;
using ePicSearch.Services;
using ePicSearch.Entities;

namespace ePicSearch.Views
{
    public partial class NewAdventurePage : ContentPage
    {
        private readonly AdventureManager _adventureManager;
        private readonly AudioPlayerService _audioPlayerService;

        public NewAdventurePage(AdventureManager adventureManager, AudioPlayerService audioPlayerService)
        {
            InitializeComponent();
            _adventureManager = adventureManager;
            this._audioPlayerService = audioPlayerService;
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

            await Navigation.PushAsync(new CameraPage(adventureData, _adventureManager, _audioPlayerService));
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