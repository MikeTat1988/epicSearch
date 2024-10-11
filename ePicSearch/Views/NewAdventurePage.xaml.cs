using ePicSearch.Services;
using ePicSearch.Entities;

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

            string adventures = Preferences.Get("Adventures", string.Empty);
            var adventureSet = new HashSet<string>(adventures.Split(';').Where(a => !string.IsNullOrEmpty(a)));

            // Check if the adventure name already exists in the set
            if (!adventureSet.Add(adventureName)) // Returns false if the name already exists
            {
                await DisplayAlert("Oops!", "This adventure name already exists. Please choose a different name.", "OK");
                return;
            }

            bool keepTakingPhotos = true;
            bool isError = false;
            int photoNumber = 1;

            var existingPhotos = _photoManager.GetPhotosForAdventure(adventureName);
            if (existingPhotos != null && existingPhotos.Count > 0)
            {
                // Resume from the next photo number if photos exist already
                photoNumber = existingPhotos.Max(p => p.SerialNumber) + 1; 
            }

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

            adventureSet.Add(adventureName);
            Preferences.Set("Adventures", string.Join(";", adventureSet));

            await DisplayAlert($"Adventure {adventureName} Saved", null, "OK");
            await Navigation.PopAsync();
        }

        private async Task HandleError(object sender, EventArgs e)
        {
            bool continueProcess = await DisplayAlert("Error", "Failed to complete the process. Your progress has been saved. Do you want to continue?", "Yes", "No");

            if (continueProcess)
            {
                // Continue the process by calling OnStartCreatingClicked again
                OnStartCreatingClicked(sender, e);
            }
            else
            {
                await Navigation.PopAsync();
            }
        }
    }
}
