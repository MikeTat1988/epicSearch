namespace ePicSearch.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void OnCleanLogsClicked(object sender, EventArgs e)
        {
            var logFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, "logs.txt");

            try
            {
                // Open the file and overwrite it with an empty string
                File.WriteAllText(logFilePath, string.Empty);
                DisplayAlert("Success", "Log file has been cleared.", "OK");
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Failed to clear log: {ex.Message}", "OK");
            }
        }
    }
}