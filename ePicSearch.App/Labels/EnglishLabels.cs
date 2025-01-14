namespace ePicSearch.Labels;

public static class EnglishLabels
{
    public static readonly Dictionary<string, string> NewAdventurePageTutorialMessages = new()
    {
        { "AdventureNameEntry", "Enter the name of your adventure here." },
        { "StartCreatingButton", "Click here to start your adventure!" }
    };

    public static readonly Dictionary<string, string> CameraPageTreasureTutorialMessages = new()
    {
        { "TreasureCodeLabel", "Write down the code amd hide it" },
        { "TreasureNextClueButton", "Long press to continue and take general photo of the hiding spot" }
    };

    public static readonly Dictionary<string, string> CameraPageClueTutorialMessages = new()
    {
        { "ClueCodeLabel", "To resume creating, write down the code and hide it" },
        { "ClueNextButton", "Long press to continue and take general photo of the hiding spot" },
        { "FinishAdventureButton", "Press to finilize the adventure" }
    };

    public static readonly string LongPressMessage = "Long press!";

    public static readonly string MuteLabel = "Mute";
    public static readonly string StartVideoLabel = "Start Video";
    public static readonly string TutorialLabel = "Tutorial";
    public static readonly string ClearLogsLabel = "Clear Logs";
}
