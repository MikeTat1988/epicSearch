using ePicSearch.Services;

namespace ePicSearch.Views
{
    public partial class NewAdventurePage : ContentPage
    {
        private readonly PhotoManager _photoManager;

        public NewAdventurePage()
        {
            InitializeComponent();
            _photoManager = new PhotoManager(new PhotoStorageService(), new CodeGenerator());
        }

        private async void OnStartCreatingClicked(object sender, EventArgs e)
        {
            string adventureName = AdventureNameEntry.Text;

            if (string.IsNullOrEmpty(adventureName))
            {
                ErrorMessage.Text = "Please enter a name for your adventure.";
                ErrorMessage.IsVisible = true;
                return;
            }

            bool keepTakingPhotos = true;
            while (keepTakingPhotos)
            {
                if (MediaPicker.IsCaptureSupported)
                {
                    try
                    {
                        FileResult? photo = await MediaPicker.CapturePhotoAsync();

                        if (photo == null)
                        {
                            await DisplayAlert("No Photo", null, "OK");
                            return;
                        }

                        var photoInfo = await _photoManager.CapturePhoto(photo, adventureName);

                        if (photoInfo.SerialNumber == 1)
                        {
                            await DisplayAlert("Treasure Photo Saved!", $"Code: {photoInfo.Code}, go hide it!", "OK");
                        }
                        else
                        {
                            await DisplayAlert($"Clue photo Saved!", $"Code: {photoInfo.Code}, go hide it!", "OK");
                        }

                        keepTakingPhotos = await DisplayAlert("Another clue?", null, "Yes", "No");

                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Camera is not supported", "OK");
                    break;
                }
            }

            SaveAdventure(adventureName);

            await DisplayAlert($"Adventure {adventureName} Saved", null, "OK");
            await Navigation.PopAsync();
        }

        private static void SaveAdventure(string adventureName)
        {
            string adventures = Preferences.Get("Adventures", string.Empty);

            // Ensure adventures separation
            if (!string.IsNullOrEmpty(adventures))
            {
                adventures += ";";
            }

            adventures += $"{adventureName};";
            Preferences.Set("Adventures", adventures);
        }
    }
}
