namespace ePicSearch.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        private async void OnMyAdventuresClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyAdventuresPage());
        }

        private async void OnCreateNewAdventureClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewAdventurePage());
        }

        private void OnQuitClicked(object sender, EventArgs e)
        {
            Application.Current.Quit();
        }
    }

}
