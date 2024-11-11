using ePicSearch.Entities;
using ePicSearch.Helpers;
using ePicSearch.Infrastructure.Services;
using ePicSearch.Services;
using Microsoft.Extensions.Logging;

namespace ePicSearch.Views
{
    public partial class MyAdventuresPage : ContentPage
    {
        private readonly AdventureManager _adventureManager;
        private readonly ILogger<MainPage> _logger;

        private readonly AudioPlayerService _audioPlayerService;

        public MyAdventuresPage(AdventureManager photoManager, ILogger<MainPage> logger, AudioPlayerService audioPlayerService)
        {
            InitializeComponent();
            _adventureManager = photoManager;
            _logger = logger;
            _audioPlayerService = audioPlayerService;
            LoadAdventures();
        }

        private void LoadAdventures()
        {
            _logger.LogInformation($"Attempting to load all adventures");

            var adventures = _adventureManager.GetAllAdventureNames();

            if (adventures.Count > 0)
            {
                AdventuresList.ItemsSource = adventures;
                AdventuresList.IsVisible = true;
                DeleteAllButton.IsVisible = true;
                StoneBG.IsVisible = true;
                NoAdventuresGrid.IsVisible = false;
            }
            else
            {
                AdventuresList.IsVisible = false;
                DeleteAllButton.IsVisible = false;
                StoneBG.IsVisible = false;
                NoAdventuresGrid.IsVisible = true;
            }
            _logger.LogInformation($"Done loading adventures");
        }

        private async void OnPlayAdventureClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.CommandParameter is string adventureName)
            {
                var parentContainer = button.Parent as VisualElement;

                ClickButton(parentContainer);

                _logger.LogInformation($"Attempting to play adventure {adventureName}");

                await Navigation.PushAsync(new GamePage(adventureName, _logger, _adventureManager, _audioPlayerService));
            }
        }

        private async void OnDeleteAdventureClicked(object sender, EventArgs e)
        {
            
            if (sender is ImageButton button && button.CommandParameter is string adventureName)
            {
                var parentContainer = button.Parent as VisualElement;

                ClickButton(parentContainer);

                bool confirm = await DisplayAlert($"Confirm delete {adventureName}", null, "Yes", "No");
                if (confirm)
                {
                    try
                    {
                        _logger.LogInformation($"Attempting to delete adventure: {adventureName}");

                        // Delete all photos and adventure folder
                        bool photosDeleted = await _adventureManager.DeleteAdventureAsync(adventureName);

                        if (photosDeleted)
                        {
                            _logger.LogInformation($"Successfully deleted adventure: {adventureName}");
                        }
                        else
                        {
                            _logger.LogWarning($"Failed to delete photos for adventure: {adventureName}");
                            await DisplayAlert("Error", "Could not delete the photos for the adventure.", "OK");
                        }

                        LoadAdventures();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error deleting adventure: {adventureName}");
                        await DisplayAlert("Error", $"Failed to delete adventure {adventureName}: {ex.Message}", "OK");
                    }
                }
            }
        }


        private async void OnDeleteAllAdventuresClicked(object sender, EventArgs e)
        {
            await AnimationHelper.AnimatePress((View)sender);

            bool confirm = await DisplayAlert("Confirm Delete All", "Are you sure you want to delete all adventures?", "Yes", "No");
            if (confirm)
            {
                try
                {
                    _logger.LogInformation("Attempting to delete all adventures.");

                    var allAdventureNames = _adventureManager.GetAllAdventureNames();
                    bool allDeleted = true;

                    foreach (var adventureName in allAdventureNames)
                    {
                        bool success = await _adventureManager.DeleteAdventureAsync(adventureName);
                        if (!success)
                        {
                            _logger.LogWarning($"Failed to delete adventure: {adventureName}");
                            allDeleted = false;
                        }
                    }

                    if (allDeleted)
                    {
                        _logger.LogInformation("All adventures successfully deleted.");
                        await DisplayAlert("Success", "All adventures have been deleted.", "OK");
                    }
                    else
                    {
                        _logger.LogWarning("Some adventures could not be deleted.");
                        await DisplayAlert("Warning", "Some adventures could not be deleted properly.", "OK");
                    }

                    LoadAdventures();  // Refresh the UI
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting all adventures.");
                    await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                }
            }
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            ClickButton((View)sender);

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