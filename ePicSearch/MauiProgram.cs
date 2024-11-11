using Microsoft.Extensions.Logging;
using ePicSearch.Infrastructure.Services;
using ePicSearch.Views;
using ePicSearch.Infrastructure.Entities.Interfaces;
using ePicSearch.Services;
using Serilog;
using CommunityToolkit.Maui;
using ePicSearch.Platforms.Android;
using ePicSearch.Helpers;

namespace ePicSearch
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            var logFilePath = Path.Combine(FileSystem.AppDataDirectory, "logs.txt");
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()  // Set minimum log level
            .Enrich.FromLogContext() 
            .WriteTo.File(
                logFilePath,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff } [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                rollOnFileSizeLimit: true,  // Roll over when file size exceeds the limit
                fileSizeLimitBytes: 10 * 1024 * 1024,  // 10 MB file size limit
                retainedFileCountLimit: 1)  // Keep only the last log file
            .CreateLogger();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureMauiHandlers(handlers =>
                {
                    // Register the custom handler for ImageButton
                    handlers.AddHandler<NoRippleImageButton, NoRippleImageButtonHandler>();
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("LuckiestGuy-Regular.ttf", "LuckiestGuy"); 

                });

            // Register services with the app data directory
            builder.Services
                .AddSingleton<IFileSystemService, FileSystemService>()
                .AddSingleton<DataStorageService>()
                .AddSingleton<PhotoStorageService>()
                .AddSingleton<CodeGenerator>()
                .AddSingleton<AdventureManager>()
                .AddSingleton<MainPage>()
                .AddSingleton<AudioPlayerService>()
                .AddSingleton<AppShell>();

            builder.Logging.AddSerilog();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
