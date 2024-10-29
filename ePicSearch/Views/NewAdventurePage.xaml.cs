using ePicSearch.Infrastructure.Services;
using ePicSearch.Infrastructure.Entities.Interfaces;
using ePicSearch.Entities;
using ePicSearch.Infrastructure.Entities;
using ePicSearch.Helpers;

namespace ePicSearch.Views
{
    public partial class NewAdventurePage : ContentPage
    {
        private readonly AdventureManager _adventureManager;

        public NewAdventurePage(AdventureManager adventureManager)
        {
            InitializeComponent();
            _adventureManager = adventureManager;
        }

        private async void OnStartCreatingClicked(object sender, EventArgs e)
        {
            await AnimationHelper.AnimatePress((View)sender);

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

            await Navigation.PushAsync(new CameraPage(adventureData, _adventureManager));
        }

        private async Task<string?> GetValidAdventureNameAsync()
        {
            string adventureName = AdventureNameEntry.Text;

            if (string.IsNullOrEmpty(adventureName))
            {
                ErrorMessage.Text = "Please enter a name for your adventure.";
                ErrorMessage.IsVisible = true;
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
            await AnimationHelper.AnimatePress((View)sender);

            if (Navigation.NavigationStack.Count > 1)
            {
                await Navigation.PopAsync();
            }
        }
    }
}