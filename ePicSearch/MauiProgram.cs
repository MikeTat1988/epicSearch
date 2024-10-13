using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using ePicSearch.Core.Services;
using ePicSearch.Views;
using ePicSearch.Services; // For PhotoManager, CodeGenerator
using Microsoft.Maui.Controls;

namespace ePicSearch
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Get the app data directory from MAUI
            string appDataDirectory = FileSystem.AppDataDirectory;

            // Register services with the app data directory
            builder.Services
                .AddSingleton<JsonStorageService>()
                .AddSingleton<PhotoStorageServiceCore>(sp => new PhotoStorageServiceCore(appDataDirectory))
                .AddSingleton<CodeGenerator>()
                .AddSingleton<PhotoManager>()
                .AddSingleton<MainPage>()
                .AddSingleton<AppShell>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
