using ePicSearch.Infrastructure.Services;

namespace ePicSearch.Helpers
{
    public class CrashLogHelper
    {
        private const string LogFileName = "logs.txt";
        private readonly AdventureManager _adventureManager;

        public CrashLogHelper(AdventureManager adventureManager)
        {
            _adventureManager = adventureManager;
        }

        public async Task HandleAppCrashAsync()
        {
            bool crashedLastRun = _adventureManager.GetCrashFlag();

            Serilog.Log.Warning($"flag value is {crashedLastRun}");
            if (crashedLastRun)
            {
                var mainPage = Application.Current?.MainPage;
                if (mainPage == null)
                {
                    Serilog.Log.Warning("MainPage is null; cannot display crash alert.");
                    return;
                }

                bool sendLogs = await mainPage.DisplayAlert(
                    "Crash Detected",
                    "The app crashed last time. Do you want to send the crash logs to help us improve?",
                    "Yes",
                    "No");

                if (sendLogs)
                {
                    await SendCrashLogsAsync();
                }

                _adventureManager.SetCrashFlag(false);
            }
        }

        public async Task SendCrashLogsAsync()
        {
            string logFilePath = Path.Combine(FileSystem.AppDataDirectory, LogFileName);

            var mainPage = Application.Current?.MainPage;
            if (mainPage == null)
            {
                Serilog.Log.Warning("MainPage is null; cannot display alerts.");
                return;
            }

            if (!File.Exists(logFilePath))
            {
                await mainPage.DisplayAlert("Error", "Crash log file not found.", "OK");
                return;
            }

            var current = Connectivity.Current.NetworkAccess;
            if (current != NetworkAccess.Internet)
            {
                bool retry = await mainPage.DisplayAlert(
                    "No Internet",
                    "Cannot send logs as there is no internet connection. Would you like to try again later?",
                    "Yes",
                    "No");

                if (retry)
                {
                    await SendCrashLogsAsync();
                }
                return;
            }

            try
            {
                var message = new EmailMessage
                {
                    Subject = "Crash Logs for ePicSearch",
                    Body = "Please find attached the crash logs.",
                    To = new List<string> { "michaeltatev@gmail.com" }, //TODO: update the mail
                };

                var attachment = new EmailAttachment(logFilePath, "logs.txt");
                if (message.Attachments != null)
                {
                    message.Attachments.Add(attachment);
                }
                else
                {
                    Serilog.Log.Warning("Attachments collection is null; cannot add log file attachment.");
                }

                await Email.Default.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException)
            {
                await mainPage.DisplayAlert("Error", "Email is not supported on this device.", "OK");
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Failed to send crash logs.");
                await mainPage.DisplayAlert("Error", "An error occurred while trying to send logs.", "OK");
            }
        }
    }
}
