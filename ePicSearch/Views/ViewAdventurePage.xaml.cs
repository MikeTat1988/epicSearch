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
                // Reverse and prepare the photos
                var preparedPhotos = PreparePhotos(photos);

                // Bind the ordered list of photos to the ListView
                PhotoListView.ItemsSource = preparedPhotos;
            }
            else
            {
                DisplayAlert("No Photos", "No photos found for this adventure.", "OK");
            }
        }

        private static List<PhotoInfo> PreparePhotos(List<PhotoInfo> photos)
        {
            // Reverse the order
            var orderedPhotos = photos.OrderByDescending(p => p.SerialNumber).ToList();

            // Hide the code for the first photo (latest clue)
            if (orderedPhotos.Count > 0)
            {
                orderedPhotos[0].ShowCode = false;
                orderedPhotos[0].DisplaySerialNumber = "1"; 
            }

            for (int i = 1; i < orderedPhotos.Count; i++)
            {
                orderedPhotos[i].ShowCode = true;
                if (i == orderedPhotos.Count - 1)  // If it's the last item (Treasure photo)
                {
                    orderedPhotos[i].DisplaySerialNumber = "Treasure!"; 
                }
                else
                {
                    orderedPhotos[i].DisplaySerialNumber = (i + 1).ToString(); 
                }
            }

            return orderedPhotos;
        }
    }
}
