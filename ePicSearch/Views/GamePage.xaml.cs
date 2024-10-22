using ePicSearch.Entities;
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
        public ObservableCollection<PhotoDisplayInfo> Photos { get; private set; }
        public string AdventureName { get; private set; }

        private readonly ILogger<MainPage> _logger;
        private readonly AdventureManager _photoManager;
        private PhotoDisplayInfo? _selectedPhoto;
        private readonly Random _random = new();

        public GamePage(string adventureName, ILogger<MainPage> logger, AdventureManager photoManager)
		{
			InitializeComponent();

            _logger = logger;
            AdventureName = adventureName;
			Photos = new ObservableCollection<PhotoDisplayInfo>();

			ShowPhotoCommand = new Command<PhotoDisplayInfo>(ShowPhoto);
			UnlockPhotoCommand = new Command<string>(UnlockPhoto);
            CloseModalCommand = new Command(CloseModal);

            _photoManager = photoManager;
            _selectedPhoto = null;

            LoadAdventurePhotos(adventureName);
            BindingContext = this;
            _logger.LogInformation($"GamePage for adventure: {adventureName} initialized");
        }

		private void LoadAdventurePhotos(string adventureName)
		{
            var photos = _photoManager.GetPhotosForAdventure(adventureName);

            _logger.LogInformation($"Photos for adventure: {adventureName} were loaded : {photos}");

            if (photos != null && photos.Count > 0)
            {
                // Order photos by SerialNumber descending (latest first)
                var orderedPhotos = photos.OrderByDescending(p => p.SerialNumber).ToList();

                Photos = new ObservableCollection<PhotoDisplayInfo>(
                    orderedPhotos.Select((photo, index) =>
                    new PhotoDisplayInfo(photo,
                                     _random.Next(-15, 15),
                                     index,
                                     orderedPhotos.Count)));

                RefreshPhotoView();

                _logger.LogInformation($"All photos for adventure: {adventureName} were loaded into Observable Collection");
            }
            else
            {
                _logger.LogWarning($"No photos were found for adventure: {adventureName}");
                DisplayAlert("No Photos", "No photos found for this adventure.", "OK");
            }

        }
        private void ShowPhoto(PhotoDisplayInfo photoInfo)
        {
            _logger.LogInformation($"A photo {photoInfo.ToString} was pressed");

            _selectedPhoto = photoInfo;

            // Set the selected photo to be displayed in the full-screen modal
            if (photoInfo.Photo.IsLocked)
            {
                // Show the locked overlay image
                FullScreenPhoto.Source = "question_mark_1.webp";
                CodeEntryOverlay.IsVisible = true;
            }
            else
            {
                FullScreenPhoto.Source = photoInfo.Photo.FilePath;
                CodeEntryOverlay.IsVisible = false;
            }

            // Display the modal
            PhotoModal.IsVisible = true;
        }

        private void UnlockPhoto(string code)
		{
            _logger.LogInformation($"A code {code} was proposed to unlock a photo");

            if (_selectedPhoto == null)
            {
                _logger.LogWarning("No photos were selected");
                return;
            }

            if (_selectedPhoto.Photo.Code == code)
            {
                _selectedPhoto.Photo.IsLocked = false;

                //updating the xaml modal view
                FullScreenPhoto.Source = _selectedPhoto.Photo.FilePath;
                CodeEntryOverlay.IsVisible = false;

                RefreshPhotoView();

                if (_photoManager.UpdatePhotoState(_selectedPhoto.Photo))
                {
                    _logger.LogInformation($"The photo with {code} was successfully updated in memory");
                }
                else
                {
                    _logger.LogError($"{_selectedPhoto} failed to be updated in memory");

                    DisplayAlert("Save Error", "Failed to save the changes. Please try again.", "OK");

                    // Revert the change in the UI to keep things consistent
                    _selectedPhoto.Photo.IsLocked = true;
                    RefreshPhotoView();
                } 
            }
            else
            {
                _logger.LogInformation($"Thecode for {_selectedPhoto} was wrong");
                DisplayAlert("Incorrect Code", "The code you entered is incorrect.", "OK");

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
        }

    }
}