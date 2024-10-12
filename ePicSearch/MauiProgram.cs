using Microsoft.Extensions.Logging;
using ePicSearch.Services;
using ePicSearch.Views;

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
                })
                .Services.AddSingleton<JsonStorageService>()
                    .AddSingleton<PhotoManager>()
                    .AddSingleton<PhotoStorageService>()
                    .AddSingleton<CodeGenerator>()
                    .AddSingleton<MainPage>()
                    .AddSingleton<AppShell>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
