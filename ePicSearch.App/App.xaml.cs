using ePicSearch.Infrastructure.Services;

namespace ePicSearch
{
    public partial class App : Application
    {
        private readonly AdventureManager _adventureManager;
        private const string CrashFlagKey = "AppCrashedLastRun";

        public App(AppShell appShell, AdventureManager adventureManager)
        {
            InitializeComponent();
            
            _adventureManager = adventureManager;
            MainPage = appShell;

            Task.Run(async () => await _adventureManager.RemoveInvalidAdventuresAsync());

            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
            TaskScheduler.UnobservedTaskException += HandleUnobservedTaskException;
        }

        private void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Log and reset to main page
            var exception = e.ExceptionObject as Exception;
            LogUnhandledException(exception);
            SetCrashFlag();
        }

        private void HandleUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            // Log and prevent crash due to unobserved task exceptions
            LogUnhandledException(e.Exception);
            SetCrashFlag();
            e.SetObserved();
        }

        private void LogUnhandledException(Exception? exception)
        {
            if (exception != null)
            {
                Serilog.Log.Error(exception, "Unhandled exception occurred");
            }
        }

        private void SetCrashFlag()
        {
            Serilog.Log.Warning("SETTING THE CRASH FLAG TRUE");
            _adventureManager.SetCrashFlag(true);
        }

        // Sync cache when the app goes to sleep (background or closing)
        protected override void OnSleep()
        {
            _adventureManager.SyncCache();
            base.OnSleep();
        }
    }
}

