using ePicSearch.Entities;
using ePicSearch.Services;

namespace ePicSearch.Views
{
    public partial class ViewAdventurePage : ContentPage
    {
        private readonly PhotoManager _photoManager;

        public ViewAdventurePage(string adventureName)
        {
            InitializeComponent();
            _photoManager = new PhotoManager(new PhotoStorageService(), new CodeGenerator());

            AdventureNameLabel.Text = adventureName;

            LoadPhotos(adventureName);
        }

        private void LoadPhotos(string adventureName)
        {
            var photos = _photoManager.GetPhotosForAdventure(adventureName);

            if (photos != null && photos.Count > 0)
            {
                // Reverse the order of photos so the latest comes first
                var orderedPhotos = photos.OrderByDescending(p => p.SerialNumber).ToList();

                PhotoListView.ItemsSource = orderedPhotos;
            }
            else
            {
                DisplayAlert("No Photos", "No photos found for this adventure.", "OK");
            }
        }

        // Method to control how the serial number and code are displayed in the UI
        private string GetDisplaySerialNumber(PhotoInfo photo, int index, int totalPhotos)
        {
            if (index == totalPhotos - 1)
                return "Treasure!";

            return (index + 1).ToString();
        }

        private bool ShouldShowCode(int index) => index != 0;
    }
}
