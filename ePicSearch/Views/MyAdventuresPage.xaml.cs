using Microsoft.Maui.Storage;
using System.Linq;

namespace ePicSearch.Views
{
    // partial keyword allows the XAML file and its code-behind file to be combined into a single class definition.
    // if I remove it The compiler will not be able to combine the two files into a single class, leading to errors.
    public partial class MyAdventuresPage : ContentPage
    {
        public MyAdventuresPage()
        {
            InitializeComponent();
            LoadAdventures();
        }

        private void LoadAdventures()
        {
            string adventures = Preferences.Get("Adventures", string.Empty);

            if (!string.IsNullOrEmpty(adventures))
            {
                var adventureList = adventures.Split(';').Where(a => !string.IsNullOrEmpty(a)).ToList();
                AdventuresList.ItemsSource = adventureList;

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

        private async void OnAdventureSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            if (e.SelectedItem is string adventureName)
            {
                await DisplayAlert($"Adventure {adventureName} Selected", null, "OK");
            }

            // Deselecting the item after handling is done
            ((ListView)sender).SelectedItem = null;
        }

        private async void OnViewAdventureClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string adventureName)
            {
                await Navigation.PushAsync(new ViewAdventurePage(adventureName));
            }
        }

        private async void OnDeleteAdventureClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string adventureName)
            {
                bool confirm = await DisplayAlert($"Confirm delete {adventureName}", null, "Yes", "No");
                if (confirm)
                {
                    string adventures = Preferences.Get("Adventures", string.Empty);
                    var adventureList = adventures.Split(';').Where(a => !a.Equals(adventureName)).ToList();
                    Preferences.Set("Adventures", string.Join(";", adventureList));

                    LoadAdventures();
                }
            }
        }

        private async void OnDeleteAllAdventuresClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirm Delete All", null, "Yes", "No");
            if (confirm)
            {
                Preferences.Set("Adventures", string.Empty);
                LoadAdventures();
            }
        }

    }
}