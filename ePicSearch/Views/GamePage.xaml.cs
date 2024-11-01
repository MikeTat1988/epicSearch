using ePicSearch.Infrastructure.Entities;
using ePicSearch.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ePicSearch.Views
{
	public partial class GamePage : ContentPage
	{
		public ICommand ShowPhotoCommand { get; }
        public ICommand UnlockPhotoCommand { get; }
        public ICommand CloseModalCommand { get; }
        public ObservableCollection<PhotoInfo> Photos { get; private set; }
        public ObservableCollection<object> BackgroundScrolls { get; private set; }
        public string AdventureName { get; private set; }

        private readonly ILogger<MainPage> _logger;
        private readonly AdventureManager _photoManager;
        private PhotoInfo? _selectedPhoto;

        public GamePage(string adventureName, ILogger<MainPage> logger, AdventureManager photoManager)
		{
			InitializeComponent();

            _logger = logger;
            AdventureName = adventureName;
			Photos = new ObservableCollection<PhotoInfo>();
            BackgroundScrolls = new ObservableCollection<object>();

            ShowPhotoCommand = new Command<PhotoInfo>(ShowPhoto);
			UnlockPhotoCommand = new Command<string>(UnlockPhoto);
            CloseModalCommand = new Command(CloseModal);

            _photoManager = photoManager;
            _selectedPhoto = null;

            LoadAdventurePhotos(adventureName);
            BindingContext = this;
            SizeChanged += GamePage_SizeChanged;
            _logger.LogInformation($"GamePage for adventure: {adventureName} initialized");
        }

        private void GamePage_SizeChanged(object sender, EventArgs e)
        {
            double pageHeight = this.Height;
            CalculateAndPopulateBackgroundScrolls(pageHeight);
        }

        private void LoadAdventurePhotos(string adventureName)
		{
            var photos = _photoManager.GetPhotosForAdventure(adventureName);

            _logger.LogInformation($"Photos for adventure: {adventureName} were loaded : {photos}");

            if (photos != null && photos.Count > 0)
            {
                // Order photos by SerialNumber descending (latest first)
                var orderedPhotos = photos.OrderByDescending(p => p.SerialNumber).ToList();

                Photos = new ObservableCollection<PhotoInfo>(photos.OrderByDescending(p => p.SerialNumber));

                Photos[Photos.Count - 1].ShowArrow = false;

                RefreshPhotoView();

                _logger.LogInformation($"All photos for adventure: {adventureName} were loaded into Observable Collection");
            }
            else
            {
                _logger.LogWarning($"No photos were found for adventure: {adventureName}");
                DisplayAlert("No Photos", "No photos found for this adventure.", "OK");
            }

        }

        private void CalculateAndPopulateBackgroundScrolls(double pageHeight)
        {
            BackgroundScrolls.Clear();

            double topHeight = 100;
            double bottomHeight = 100;
            double middleTileHeight = 270;

            // Minimum number of middle tiles based on the number of photos
            int numberOfMiddleTiles = Photos.Count;

            // Calculate the number of middle tiles required to fill the screen
            double requiredMiddleTiles = (pageHeight - topHeight - bottomHeight) / middleTileHeight;
            int numberOfMiddleTilesRequired = (int)Math.Ceiling(requiredMiddleTiles);

            numberOfMiddleTiles = Math.Max(numberOfMiddleTiles, numberOfMiddleTilesRequired);

            for (int i = 0; i < numberOfMiddleTiles; i++)
            {
                BackgroundScrolls.Add(new BackgroundTile
                {
                    ImageSource = "scroll_middle_n.webp",
                    Height = 270 
                });
            }

            _logger.LogInformation($"added {BackgroundScrolls.Count} background scroll tiles.");
                
            
            RefreshPhotoView();
        }

        private void ShowPhoto(PhotoInfo photoInfo)
        {
            _logger.LogInformation($"A photo {photoInfo} was pressed");

            _selectedPhoto = photoInfo;

            // Set the selected photo to be displayed in the full-screen modal
            if (photoInfo.IsLocked)
            {
                // Show the locked overlay image
                FullScreenPhoto.Source = "question_mark_1.webp";
                CodeEntryOverlay.IsVisible = true;
            }
            else
            {
                FullScreenPhoto.Source = photoInfo.FilePath;
                CodeEntryOverlay.IsVisible = false;
            }

            // Display the modal
            PhotoModal.IsVisible = true;
        }

        private async void UnlockPhoto(string code)
		{
            _logger.LogInformation($"A code {code} was proposed to unlock a photo");

            if (_selectedPhoto == null)
            {
                _logger.LogWarning("No photos were selected");
                return;
            }

            // Ensure the code is 4 digits and numeric
            if (code.Length != 4 || !code.All(char.IsDigit))
            {
                await ShowOopsPopup();
                return;
            }

            if (_selectedPhoto.Code == code)
            {
                _selectedPhoto.IsLocked = false;

                //updating the xaml modal view
                FullScreenPhoto.Source = _selectedPhoto.FilePath;
                CodeEntryOverlay.IsVisible = false;

                RefreshPhotoView();

                if (_photoManager.UpdatePhotoState(_selectedPhoto))
                {
                    _photoManager.SyncCache();
                    _logger.LogInformation($"The photo with {code} was successfully updated in memory");
                }
                else
                {
                    _logger.LogError($"{_selectedPhoto} failed to be updated in memory");

                    await DisplayAlert("Save Error", "Failed to save the changes. Please try again.", "OK");

                    // Revert the change in the UI to keep things consistent
                    _selectedPhoto.IsLocked = true;
                    RefreshPhotoView();
                } 
            }
            else
            {
                _logger.LogInformation($"Thecode for {_selectedPhoto} was wrong");
                await DisplayAlert("Incorrect Code", "The code you entered is incorrect.", "OK");

            }
        }

        private async Task ShowOopsPopup()
        {
            OopsPopup.IsVisible = true;

            await OopsPopup.FadeTo(1, 250);

            await Task.Delay(1000);

            await OopsPopup.FadeTo(0, 250);
            OopsPopup.IsVisible = false;
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            if (Navigation.NavigationStack.Count > 1)
            {
                await Navigation.PopAsync();
            }
        }

        private void CloseModal()
        {
            PhotoModal.IsVisible = false;
        }

        private void RefreshPhotoView()
		{
            PhotoCollectionView.ItemsSource = null;
            PhotoCollectionView.ItemsSource = Photos;
            BackgroundView.ItemsSource = null;
            BackgroundView.ItemsSource = BackgroundScrolls;
        }

        public class ScrollMiddleInfo
        {
            public string ImageSource { get; set; } = "scroll_middle.webp";
        }

        public class BackgroundTile
        {
            public string ImageSource { get; set; }
            public double Height { get; set; }
        }
    }
}