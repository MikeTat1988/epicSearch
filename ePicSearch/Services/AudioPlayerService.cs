using Plugin.Maui.Audio;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ePicSearch.Services
{
    public class AudioPlayerService
    {
        private readonly IAudioManager _audioManager;
        private readonly object _playerLock = new object();
        private readonly ILogger<AudioPlayerService> _logger;

        public AudioPlayerService(ILogger<AudioPlayerService> logger)
        {
            _audioManager = AudioManager.Current;
            _logger = logger;
        }

        public async Task PlaySoundAsync(string audioFileName)
        {
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
