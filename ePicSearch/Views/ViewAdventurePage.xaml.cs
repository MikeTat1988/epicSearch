using ePicSearch.Entities;
using ePicSearch.Infrastructure.Services;

namespace ePicSearch.Views
{
    public partial class ViewAdventurePage : ContentPage
    {
        private readonly PhotoManager _photoManager;

        public ViewAdventurePage(string adventureName, PhotoManager photoManager)
        {
            InitializeComponent();
            _photoManager = photoManager;

            AdventureNameLabel.Text = adventureName;

            LoadPhotos(adventureName);
        }

        //TODO: maybe remove after finishing GamePAge?
        private void LoadPhotos(string adventureName)
        {
            var photos = _photoManager.GetPhotosForAdventure(adventureName);

            if (photos != null && photos.Count > 0)
            {
                // Order photos by SerialNumber descending (latest first)
                var orderedPhotos = photos.OrderByDescending(p => p.SerialNumber).ToList();

                var displayPhotos = orderedPhotos.Select((photo, index) => new PhotoDisplayInfo(photo, index, orderedPhotos.Count)).ToList();

                PhotoListView.ItemsSource = displayPhotos;
            }
            else
            {
                DisplayAlert("No Photos", "No photos found for this adventure.", "OK");
            }
        }
    }
}