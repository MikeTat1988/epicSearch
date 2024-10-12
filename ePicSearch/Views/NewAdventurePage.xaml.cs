using ePicSearch.Services;

namespace ePicSearch.Views
{
    public partial class NewAdventurePage : ContentPage
    {
        private readonly PhotoManager _photoManager;

        // Use Dependency Injection to provide PhotoManager instance
        public NewAdventurePage(PhotoManager photoManager)
        {
            InitializeComponent();
            _photoManager = photoManager;
        }

        // Handles starting the creation of a new adventure
        private async void OnStartCreatingClicked(object sender, EventArgs e)
        {
            string adventureName = AdventureNameEntry.Text;

            if (string.IsNullOrEmpty(adventureName))
            {
                ErrorMessage.Text = "Please enter a name for your adventure.";
                ErrorMessage.IsVisible = true;
                return;
            }

            var existingAdventures = _photoManager.GetPhotosForAdventure(string.Empty).Select(p => p.AdventureName).Distinct().ToList();

            if (existingAdventures.Contains(adventureName))
            {
                await DisplayAlert("Oops!", "This adventure name already exists. Please choose a different name.", "OK");
                return;
            }

            bool keepTakingPhotos = true;
            bool isError = false;

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
                            await DisplayAlert("Clue photo Saved!", $"Code: {photoInfo.Code}, go hide it!", "OK");
                        }

                        keepTakingPhotos = await DisplayAlert("Another clue?", null, "Yes", "No");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                        isError = true;
                        break;
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Camera is not supported", "OK");
                    isError = true;
                    break;
                }
            }

            if (isError)
            {
                await HandleError(sender, e);
                return;
            }

            await DisplayAlert($"Adventure {adventureName} Saved", null, "OK");
            await Navigation.PopAsync();
        }

        private async Task HandleError(object sender, EventArgs e)
        {
            bool continueProcess = await DisplayAlert("Error", "Failed to complete the process. Your progress has been saved. Do you want to continue?", "Yes", "No");

            if (continueProcess)
            {
                OnStartCreatingClicked(sender, e);
            }
            else
            {
                await Navigation.PopAsync();
            }
        }
    }
}