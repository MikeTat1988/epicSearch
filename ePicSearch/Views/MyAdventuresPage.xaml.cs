using ePicSearch.Entities;
using ePicSearch.Services;
using Microsoft.Maui.Controls;
using System.Linq;

namespace ePicSearch.Views
{
    public partial class MyAdventuresPage : ContentPage
    {
        private readonly PhotoManager _photoManager;

        // Use Dependency Injection to provide PhotoManager instance
        public MyAdventuresPage(PhotoManager photoManager)
        {
            InitializeComponent();
            _photoManager = photoManager;
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
                        var photos = _photoManager.GetPhotosForAdventure(adventureName);
                        foreach (var photo in photos)
                        {
                            _photoManager.DeletePhoto(photo);
                        }
                        LoadAdventures();
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"Failed to delete adventure {adventureName}: {ex.Message}", "OK");
                    }
                }
            }
        }

        private async void OnDeleteAllAdventuresClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirm Delete All", null, "Yes", "No");
            if (confirm)
            {
                var allAdventureNames = _photoManager.GetAllAdventureNames();
                foreach (var adventureName in allAdventureNames)
                {
                    var photos = _photoManager.GetPhotosForAdventure(adventureName);
                    foreach (var photo in photos)
                    {
                        _photoManager.DeletePhoto(photo);
                    }
                }
                LoadAdventures();
            }
        }
    }
}