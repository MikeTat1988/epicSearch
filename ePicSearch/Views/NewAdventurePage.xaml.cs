namespace ePicSearch.Views
{
    public partial class NewAdventurePage : ContentPage
    {
        public NewAdventurePage()
        {
            InitializeComponent();
        }

        private async void OnStartAdventureClicked(object sender, EventArgs e)
        {
            string adventureName = AdventureNameEntry.Text;

            if (string.IsNullOrEmpty(adventureName))
            {
                ErrorMessage.Text = "Please enter a name for your adventure.";
                ErrorMessage.IsVisible = true;
                return;
            }

            //Create and then Save the new adventure to MAUI Preferences - a small keyvalue store for persistency
            string currentAdventures = Preferences.Get("Adventures", string.Empty);
            Preferences.Set("Adventures", currentAdventures + ";" + adventureName);

            await DisplayAlert("Adventure Created", $"New adventure '{adventureName}' created!", "OK");

            // go back to the previous page in a navigation stack.
            await Navigation.PopAsync();
        }
    }
}