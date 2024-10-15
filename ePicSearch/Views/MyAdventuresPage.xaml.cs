using ePicSearch.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace ePicSearch.Views
{
    public partial class MyAdventuresPage : ContentPage
    {
        private readonly PhotoManager _photoManager;
        private readonly ILogger<MainPage> _logger;

        // Use Dependency Injection to provide PhotoManager instance
        public MyAdventuresPage(PhotoManager photoManager, ILogger<MainPage> logger)
        {
            InitializeComponent();
            _photoManager = photoManager;
            _logger = logger;
            LoadAdventures();
        }

        // Load adventures from JSON to display in the UI
        private void LoadAdventures()
        {
            var adventures = _photoManager.GetAllAdventureNames();

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
        }

        private async void OnViewAdventureClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string adventureName)
            {
                await Navigation.PushAsync(new ViewAdventurePage(adventureName, _photoManager));
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
                        bool photosDeleted = _photoManager.DeleteAdventure(adventureName);

                        if (photosDeleted)
                        {
                            _logger.LogInformation($"Successfully deleted adventure: {adventureName}");
                            LoadAdventures();
                        }
                        else
                        {
                            _logger.LogWarning($"Failed to delete photos for adventure: {adventureName}");
                            await DisplayAlert("Error", "Could not delete the photos for the adventure.", "OK");
                        }
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

                    var allAdventureNames = _photoManager.GetAllAdventureNames();
                    bool allDeleted = true;

                    foreach (var adventureName in allAdventureNames)
                    {
                        bool success = _photoManager.DeleteAdventure(adventureName);
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

    }
}