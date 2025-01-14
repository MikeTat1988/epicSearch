namespace ePicSearch.Labels;

public static class TutorialMessages
{
    public static readonly Dictionary<string, string> NewAdventurePageMessages = new()
    {
        { "AdventureNameEntry", "Enter the name of your adventure here." },
        { "StartCreatingButton", "Click here to start your adventure!" }
    };

    public static readonly Dictionary<string, string> CameraPageTreasureMessages = new()
    {
        { "TreasureCodeLabel", "Write down the code amd hide it" },
        { "TreasureNextClueButton", "Long press to continue and take general photo of the hiding spot" }
    };

    public static readonly Dictionary<string, string> CameraPageClueMessages = new()
    {
        { "ClueCodeLabel", "To resume creating, write down the code and hide it" },
        { "ClueNextButton", "Long press to continue and take general photo of the hiding spot" },
        { "FinishAdventureButton", "Press to finilize the adventure" }
    };
}
