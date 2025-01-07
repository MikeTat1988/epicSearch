using CommunityToolkit.Maui.Views;

namespace ePicSearch.Views
{
    public partial class StartupVideoPage : ContentPage
    {
        public StartupVideoPage()
        {
            InitializeComponent();
        }

        private void NavigateToShell()
        {
            var serviceProvider = MauiProgram.AppInstance.Services;
            var shell = serviceProvider.GetRequiredService<AppShell>();
            Application.Current.MainPage = shell;
        }

        private void OnSkipButtonClicked(object sender, EventArgs e)
        {
            NavigateToShell();
        }

        private void StartupVideo_MediaEnded(object sender, EventArgs e)
        {
            NavigateToShell();
        }
    }
}
