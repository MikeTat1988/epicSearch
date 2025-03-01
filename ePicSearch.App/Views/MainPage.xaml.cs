﻿using ePicSearch.Behaviors;
using ePicSearch.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using ePicSearch.Helpers;
using ePicSearch.Services;
using ePicSearch.Entities;

namespace ePicSearch.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly AdventureManager _adventureManager;
        private readonly AudioPlayerService _audioPlayerService;
        private readonly AdventureNameGenerator _nameGenerator;
        private readonly ILogger<MainPage> _logger;
        private readonly CrashLogHelper _crashLogHelper;
        private bool _isBlurred = false;

        public MainPage(AdventureManager adventureMAnager, ILogger<MainPage> logger, AudioPlayerService audioPlayerService, AdventureNameGenerator nameGenerator)
        {
            InitializeComponent();
            _adventureManager = adventureMAnager;
            _logger = logger;
            _crashLogHelper = new CrashLogHelper(_adventureManager);
            _audioPlayerService = audioPlayerService;
            _nameGenerator = nameGenerator;

            Appearing += MainPage_Appearing;
            _logger.LogInformation("MainPage initialized.");
        }

        private async void MainPage_Appearing(object? sender, EventArgs e)
        {
            await _crashLogHelper.HandleAppCrashAsync();

            if (!_isBlurred)
            {
                // Display the background for 1 second
                await Task.Delay(1000);

                var zoomTask = BackgroundImage.ScaleTo(1.1, 2000, Easing.CubicInOut);

                var blurBehavior = BackgroundImage.Behaviors.OfType<BlurBehavior>().FirstOrDefault();
                var blurTask = blurBehavior?.AnimateBlurEffect(10, 100);
                var fadeTask = BackgroundImage.FadeTo(0.6, 2000, Easing.CubicInOut);
                var buttonsAppearTask = ButtonsAppear(MyAdventuresButton, CreateAdventureButton, SettingsButton, QuitButton, TitleImage);

                await Task.WhenAll(zoomTask, blurTask!, fadeTask, buttonsAppearTask);

                _isBlurred = true;
            }

            await CheckForIncompleteAdventure();
        }

        private async Task ButtonsAppear(params NoRippleImageButton[] buttons)
        {
            var animationTasks = new List<Task>();

            foreach (var button in buttons)
            {
                var buttonTask = Task.WhenAll(
                    button.FadeTo(1, 1000),
                    button.TranslateTo(0, -10, 1000, Easing.CubicInOut)
                );

                animationTasks.Add(buttonTask);
            }

            await Task.WhenAll(animationTasks);
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            ClickButton(sender);

            await Navigation.PushAsync(new SettingsPage(_adventureManager));
        }

        private async void OnMyAdventuresClicked(object sender, EventArgs e)
        {
            ClickButton(sender);

            await Navigation.PushAsync(new MyAdventuresPage(_adventureManager, _logger, _audioPlayerService));
        }

        private async void OnCreateNewAdventureClicked(object sender, EventArgs e)
        {
            ClickButton(sender);

            await Navigation.PushAsync(new NewAdventurePage(_adventureManager, _audioPlayerService, _nameGenerator, _logger));
        }

        private async void OnQuitClicked(object sender, EventArgs e)
        {
            ClickButton(sender);

            Application.Current?.Quit();
        }

        private async Task CheckForIncompleteAdventure()
        {
            var incompleteAdventure = _adventureManager.GetIncompleteAdventure();

            if (incompleteAdventure != null)
            {
                _logger.LogInformation("Incomplete adventure found: " + incompleteAdventure.AdventureName);

                var resumePage = new ResumeAdventurePromptPage(_logger);
                resumePage.Initialize(incompleteAdventure, _adventureManager, _audioPlayerService);

                await Navigation.PushModalAsync(resumePage);
            }
            else
            {
                _logger.LogInformation("No incomplete adventure found");
            }
        }


        private async void ClickButton(object sender)
        {
            await _audioPlayerService.PlaySoundAsync(SoundLabels.ButtonPress);
            await AnimationHelper.AnimatePress((View)sender);
        }
    }
}
