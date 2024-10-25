using ePicSearch.Infrastructure.Services;
using ePicSearch.Infrastructure.Entities.Interfaces;
using ePicSearch.Entities;
using ePicSearch.Infrastructure.Entities;

namespace ePicSearch.Views
{
    public partial class NewAdventurePage : ContentPage
    {
        private readonly AdventureManager _adventureManager;

        public NewAdventurePage(AdventureManager adventureManager)
        {
            InitializeComponent();
            _adventureManager = adventureManager;
        }

        private async void OnStartCreatingClicked(object sender, EventArgs e)
        {
            var adventureName = await GetValidAdventureNameAsync();

            if (adventureName == null)
            {
                return; // Validation failed, exit the method.
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

                        IFileResult appFileResult = new AppFileResult(photo);
                        var photoInfo = await _adventureManager.CapturePhoto(appFileResult, adventureName);

                        if (photoInfo == null)
                        {
                            await HandleError("Failed to save the photo", sender, e);
                            return;
                        }

                        await DisplayPhotoSavedMessage(photoInfo);

                        keepTakingPhotos = await DisplayAlert("Another clue?", null, "Yes", "No");
                    }
                    catch (Exception ex)
                    {
                        await HandleError(ex.Message, sender, e);
                        return;
                    }
                }
                else
                {
                    await HandleError("Camera is not supported.", sender, e);
                    return;
                }
            }

            UnlockLastPhoto(adventureName);

            await DisplayAlert($"Adventure {adventureName} Saved", null, "OK");
            _adventureManager.SyncCache();
            await Navigation.PopAsync();
        }

        private void UnlockLastPhoto(string adventureName)
        {
            var photos = _adventureManager.GetPhotosForAdventure(adventureName);
            if (photos != null && photos.Count > 0)
            {
                var lastPhoto = photos.OrderByDescending(p => p.SerialNumber).FirstOrDefault();
                if (lastPhoto != null)
                {
                    lastPhoto.IsLocked = false;
                    _adventureManager.UpdatePhotoState(lastPhoto);
                }
            }
        }

        private async Task HandleError(string errorMessage, object sender, EventArgs e)
        {
            bool continueProcess = await DisplayAlert("Error", $"{errorMessage} Your progress has been saved. Do you want to continue?", "Yes", "No");

            if (continueProcess)
            {
                OnStartCreatingClicked(sender, e);
            }
            else
            {
                await Navigation.PopAsync();
            }
        }

        private async Task<string?> GetValidAdventureNameAsync()
        {
            string adventureName = AdventureNameEntry.Text;

            if (string.IsNullOrEmpty(adventureName))
            {
                ErrorMessage.Text = "Please enter a name for your adventure.";
                ErrorMessage.IsVisible = true;
                return null;
            }

            var existingAdventures = _adventureManager.GetAllAdventureNames();

            if (existingAdventures.Contains(adventureName, StringComparer.OrdinalIgnoreCase))
            {
                await DisplayAlert("Oops!", "This adventure name already exists. Please choose a different name.", "OK");
                return null;
            }

            return adventureName;
        }

        private async Task DisplayPhotoSavedMessage(PhotoInfo photoInfo)
        {
            if (photoInfo.SerialNumber == 1)
            {
                await DisplayAlert("Treasure Photo Saved!", $"Code: {photoInfo.Code}, go hide it!", "OK");
            }
            else
            {
                await DisplayAlert("Clue Photo Saved!", $"Code: {photoInfo.Code}, go hide it!", "OK");
            }
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            if (Navigation.NavigationStack.Count > 1)
            {
                await Navigation.PopAsync();
            }
        }
    }
}