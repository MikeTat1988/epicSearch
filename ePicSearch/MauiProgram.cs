using Microsoft.Extensions.Logging;
using ePicSearch.Infrastructure.Services;
using ePicSearch.Views;
using ePicSearch.Infrastructure.Entities.Interfaces;
using ePicSearch.Services;
using Serilog;

namespace ePicSearch
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            var logFilePath = Path.Combine(FileSystem.AppDataDirectory, "logs.txt");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    logFilePath,
                    //rollingInterval: RollingInterval.Day,  // Create a new log file each day
                    rollOnFileSizeLimit: true,  // Roll over when file size is exceeded
                    fileSizeLimitBytes: 10 * 1024 * 1024,  // 10 MB limit
                    retainedFileCountLimit: 1) // Keep only the last 1 file
                .CreateLogger();

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

            builder.Logging.AddSerilog();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
