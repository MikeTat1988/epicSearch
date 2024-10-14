using Microsoft.Extensions.Logging;
using ePicSearch.Infrastructure.Services;
using ePicSearch.Views;
using ePicSearch.Infrastructure.Entities.Interfaces;
using ePicSearch.Services;

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

            // Register services with the app data directory
            builder.Services
                .AddSingleton<IFileSystemService, FileSystemService>()
                .AddSingleton<JsonStorageService>()
                .AddSingleton<PhotoStorageService>()
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
