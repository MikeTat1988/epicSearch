using ePicSearch.Infrastructure.Services;

namespace ePicSearch
{
    public partial class App : Application
    {
        private readonly JsonStorageService _jsonStorageService;
        public App(AppShell appShell, JsonStorageService jsonStorageService)
        {
            InitializeComponent();
            MainPage = appShell;
            _jsonStorageService = jsonStorageService;
        }

        // Sync cache when the app goes to sleep (background or closing)
        protected override void OnSleep()
        {
            _jsonStorageService.SyncCacheToFile();
            base.OnSleep();
        }
    }
}

