using ePicSearch.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace ePicSearch.Services
{
    public class AudioPlayerService
    {
        private readonly IAudioManager _audioManager;
        private readonly ILogger<AudioPlayerService> _logger;
        private readonly AdventureManager _adventureManager;

        public AudioPlayerService(ILogger<AudioPlayerService> logger, AdventureManager adventureManager)
        {
            _audioManager = AudioManager.Current;
            _logger = logger;
            _adventureManager = adventureManager;
        }

        public async Task PlaySoundAsync(string audioFileName)
        {
            if (_adventureManager.IsMuted)
                return;

            try
            {
                IAudioPlayer player = null;

                var audioFile = await FileSystem.OpenAppPackageFileAsync(audioFileName);
                player = _audioManager.CreatePlayer(audioFile);
                player.Loop = false;

                // Subscribe to the PlaybackEnded event for cleanup
                player.PlaybackEnded += (sender, args) =>
                {
                    player.Dispose();
                    _logger.LogInformation($"Audio playback ended and resources disposed for {audioFileName}.");
                };

                // Start playing the audio
                player.Play();
                _logger.LogInformation($"Playing audio: {audioFileName}");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error playing audio '{audioFileName}': {ex.Message}");
            }
        }
    }
}
