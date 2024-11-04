using Plugin.Maui.Audio;
using Microsoft.Maui.Layouts;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;

namespace ePicSearch.Views
{
    public partial class TutorialPage : ContentPage
    {
        public readonly List<TutorialStep> _tutorialSteps;
        private int _currentStepIndex = -1; 
        private readonly IAudioManager _audioManager;
        private readonly Random _random = new Random();
        private readonly ILogger<MainPage> _logger;
        private double _screenWidth;
        private double _screenHeight;

        public TutorialPage(ILogger<MainPage> logger)
        {
            InitializeComponent();

            _audioManager = AudioManager.Current;
            _logger = logger;

            _tutorialSteps = InitializeTutorialSteps();

            _logger.LogInformation($"Tutorial Page initialized");

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            _screenWidth = mainDisplayInfo.Width / mainDisplayInfo.Density;
            _screenHeight = mainDisplayInfo.Height / mainDisplayInfo.Density;

            // Call AnimateNextStep directly
            AnimateFirstStep();
        }
        private async void AnimateFirstStep()
        {
            await AnimateNextStep();
        }

        public async void OnNextButtonClicked(object sender, EventArgs e)
        {
            if (_currentStepIndex < _tutorialSteps.Count - 1)
            {
                await AnimateNextStep();
            }
            else
            {
                HandleEndOfTutorial();
            }
        }

        public async void OnExitButtonClicked(object sender, EventArgs e)
        {
            _logger.LogInformation("Exit button clicked. Navigating back to Main Page.");
            await Navigation.PopAsync(); 
        }

        private List<TutorialStep> InitializeTutorialSteps()
        {
            var steps = new List<TutorialStep>();
            int totalSteps = 6;

            for (int i = 1; i <= totalSteps; i++)
            {
                // Randomize rotation between -30 and 30 degrees
                double rotation = _random.NextDouble() * 60 - 30;

                steps.Add(new TutorialStep
                {
                    ImageSource = $"tutorial_{i}.webp",
                    Rotation = rotation,
                    AudioFile = $"tutorial_{i}.mp3"
                });
            }
            return steps;
        }

        private async Task AnimateNextStep()
        {
            _currentStepIndex++;

            if (_currentStepIndex >= _tutorialSteps.Count)
            {
                // All steps completed
                _logger.LogInformation("All tutorial steps completed.");
                HandleEndOfTutorial();
                return;
            }

            var step = _tutorialSteps[_currentStepIndex];
            var image = CreateAnimatedImage(step.ImageSource);

            AbsoluteLayout.SetLayoutBounds(image, new Rect(0.5, -0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.PositionProportional);

            AnimationContainer.Children.Add(image);

            HideExistingLabels();
            var label = CreateAnimatedLabel(_currentStepIndex);

            AbsoluteLayout.SetLayoutBounds(label, new Rect(0.5, -0.3, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.PositionProportional);

            AnimationContainer.Children.Add(label);

            var deltaY = 0.2 * _screenHeight;

            var moveAnimation = image.TranslateTo(0, deltaY, 1000, Easing.CubicOut);
            var rotateAnimation = image.RotateTo(step.Rotation, 1000, Easing.CubicOut);
            var fadeInAnimation = image.FadeTo(1.0, 1000, Easing.CubicOut);

            var labelMoveAnimation = label.TranslateTo(0, deltaY + (0.6 * _screenHeight), 1000, Easing.CubicOut); // Adjust as needed
            var labelFadeInAnimation = label.FadeTo(1.0, 1000, Easing.CubicOut);

            var playSoundTask = PlaySound(step.AudioFile);

            await Task.WhenAll(moveAnimation, rotateAnimation, fadeInAnimation, playSoundTask, labelMoveAnimation, labelFadeInAnimation);

            if (_currentStepIndex == _tutorialSteps.Count - 1)
            {
                _logger.LogInformation("Last tutorial step animated. Showing Exit button.");
                HandleEndOfTutorial();
            }
        }

        private void HideExistingLabels()
        {
            var existingLabels = AnimationContainer.Children.OfType<Label>().ToList();
            foreach (var label in existingLabels)
            {
                label.IsVisible = false; 
            }
        }

        private Label CreateAnimatedLabel(int stepIndex)
        {
            return new Label
            {
                Text = TutorialTexts.GetTextForStep(stepIndex),
                TextColor = Colors.White,
                FontSize = 24,
                FontFamily = "LuckiestGuy",
                HorizontalOptions = LayoutOptions.Center,
                Opacity = 0
            };
        }

        private async Task PlaySound(string audioFileName)
        {
            try
            {
                var audioFile = await FileSystem.OpenAppPackageFileAsync(audioFileName);
                using var player = _audioManager.CreatePlayer(audioFile);
                player.Loop = false;
                player.Play();
                _logger.LogInformation($"Playing audio: {audioFileName}");

                // Wait for the audio to finish playing
                while (player.IsPlaying)
                {
                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error playing audio '{audioFileName}': {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Error playing audio '{audioFileName}': {ex.Message}");
            }
        }

        private void HandleEndOfTutorial()
        {
            _logger.LogInformation("All tutorial steps completed or last step animated. Showing Exit button.");
            NextButton.IsVisible = false;
            ExitButton.IsVisible = true;
        }

        public class TutorialStep
        {
            public string ImageSource { get; set; }
            public double Rotation { get; set; } 
            public string AudioFile { get; set; }
        }

        private Image CreateAnimatedImage(string imageSource)
        {
            var size = _screenWidth * 0.95; 

            return new Image
            {
                Source = imageSource,
                WidthRequest = size,
                HeightRequest = size,
                Aspect = Aspect.AspectFit,
                Rotation = 0,
                Opacity = 0
            };
        }


        public static class TutorialTexts
        {
            public const string Step1 = "Welcome to Step 1!";
            public const string Step2 = "This is Step 2.";
            public const string Step3 = "You're now at Step 3.";
            public const string Step4 = "Step 4 is here.";
            public const string Step5 = "Almost there! Step 5.";
            public const string Step6 = "Final Step 6. Well done!";

            public static string GetTextForStep(int stepIndex)
            {
                return stepIndex switch
                {
                    0 => Step1,
                    1 => Step2,
                    2 => Step3,
                    3 => Step4,
                    4 => Step5,
                    5 => Step6,
                    _ => string.Empty,
                };
            }
        }
    }
}
