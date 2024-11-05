using ePicSearch.Infrastructure.Services;

namespace ePicSearch
{
    public partial class App : Application
    {
        private readonly AdventureManager _adventureManager;
        public App(AppShell appShell, AdventureManager adventureManager)
        {
            InitializeComponent();
            MainPage = appShell;
            _adventureManager = adventureManager;

            Task.Run(async () => await _adventureManager.RemoveInvalidAdventuresAsync());
        }

        // Sync cache when the app goes to sleep (background or closing)
        protected override void OnSleep()
        {
            _adventureManager.SyncCache();
            base.OnSleep();
        }
    }
}

