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
                // Split the saved adventures into a list and update the ListView's ItemsSource
                AdventuresList.ItemsSource = adventures.Split(';').Where(a => !string.IsNullOrEmpty(a)).ToList();
            }

            else
            {
                // If no adventures are found, display a placeholder message
                AdventuresList.ItemsSource = new string[] { "No adventures found" };
            }
        }

        private async void OnAdventureSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            if (e.SelectedItem is string adventureName)
            {
                await DisplayAlert("Adventure Selected", $"You selected {adventureName}", "OK");
                //will await untill user presses OK
            }

            // Deselecting the item after handling is done
            ((ListView)sender).SelectedItem = null;
        }


    }
}