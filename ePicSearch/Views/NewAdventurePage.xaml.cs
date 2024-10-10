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

        private async void OnStartAdventureClicked(object sender, EventArgs e)
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
                            await DisplayAlert("No Photo", "No photo was captured.", "OK");
                            return;
                        }

                        var photoInfo = await _photoManager.CapturePhoto(photo, adventureName);

                        if (photoInfo.SerialNumber == 1)
                        {
                            await DisplayAlert("Treasure Saved!", $"The Treasure photo is saved! Code: {photoInfo.Code}, go hide it!", "OK");
                        }
                        else
                        {
                            await DisplayAlert($"Clue photo Saved!", $"A clue has been saved! Code: {photoInfo.Code}!", "OK");
                        }

                        keepTakingPhotos = await DisplayAlert("Another clue?", "Do you want to add another photo?", "Yes", "No");

                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Not Supported", "Camera is not supported on this device.", "OK");
                    break;
                }
            }

            await Navigation.PopAsync();
        }
    }
}
