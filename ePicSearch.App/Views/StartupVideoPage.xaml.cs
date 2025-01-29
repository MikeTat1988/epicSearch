using Android.Window;
using CommunityToolkit.Maui.Views;

namespace ePicSearch.Views
{
    public partial class StartupVideoPage : ContentPage
    {
        public StartupVideoPage()
        {
            InitializeComponent();
            Loaded += async (s, e) => await StartIntroSequence();
        }

        private async Task StartIntroSequence()
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await IntroText.FadeTo(1, 300);
                await Task.Delay(500);

                // Fade out slowly
                await IntroText.FadeTo(0, 1500);
                await IntroOverlay.FadeTo(0, 1000);
                IntroOverlay.IsVisible = false;

                StartupVideo.IsVisible = true;
                StartupVideo.Play();

                await Task.Delay(2000);
                SkipButton.IsVisible = true;
            });
        }

        private void NavigateToShell()
        {
            var serviceProvider = MauiProgram.AppInstance.Services;
            var shell = serviceProvider.GetRequiredService<AppShell>();
            Application.Current.MainPage = shell;
        }

        private void OnSkipButtonClicked(object sender, EventArgs e)
        {
            StartupVideo.Stop();

            NavigateToShell();
        }

        private void StartupVideo_MediaEnded(object sender, EventArgs e)
        {
            NavigateToShell();
        }
    }
}
