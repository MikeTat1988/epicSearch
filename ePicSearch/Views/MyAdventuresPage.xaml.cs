using ePicSearch.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace ePicSearch.Views
{
    public partial class MyAdventuresPage : ContentPage
    {
        private readonly AdventureManager _adventureManager;
        private readonly ILogger<MainPage> _logger;

        // Use Dependency Injection to provide PhotoManager instance
        public MyAdventuresPage(AdventureManager photoManager, ILogger<MainPage> logger)
        {
            InitializeComponent();
            _adventureManager = photoManager;
            _logger = logger;

            LoadAdventures();
        }

        // Load adventures from JSON to display in the UI
        private void LoadAdventures()
        {
            _logger.LogInformation($"Attempting to load all adventures");

            var adventures = _adventureManager.GetAllAdventureNames();

            if (adventures.Count > 0)
            {
                AdventuresList.ItemsSource = adventures;
                AdventuresList.IsVisible = true;
                DeleteAllButton.IsVisible = true;
                NoAdventuresGrid.IsVisible = false;
            }
            else
            {
                AdventuresList.IsVisible = false;
                DeleteAllButton.IsVisible = false;
                NoAdventuresGrid.IsVisible = true;
            }
            _logger.LogInformation($"Done loading adventures");
        }

        private async void OnPlayAdventureClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string adventureName)
            {
                _logger.LogInformation($"Attempting to play adventure {adventureName}");

                await Navigation.PushAsync(new GamePage(adventureName, _logger, _adventureManager));
            }
        }

        private async void OnDeleteAdventureClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string adventureName)
            {
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
            if (Navigation.NavigationStack.Count > 1)
            {
                await Navigation.PopAsync();
            }
        }
    }
}