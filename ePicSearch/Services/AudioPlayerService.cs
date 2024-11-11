using Plugin.Maui.Audio;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ePicSearch.Services
{
    public class AudioPlayerService
    {
        private readonly IAudioManager _audioManager;
        private IAudioPlayer _currentPlayer = null;
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
                if (_currentPlayer != null && _currentPlayer.IsPlaying)
                {
                    _currentPlayer.Stop();
                    _currentPlayer.Dispose();
                    _currentPlayer = null;
                }

                var audioFile = await FileSystem.OpenAppPackageFileAsync(audioFileName);
                _currentPlayer = _audioManager.CreatePlayer(audioFile);
                _currentPlayer.Loop = false;
                _currentPlayer.Play();
                _logger.LogInformation($"Playing audio: {audioFileName}");

                // Wait for audio to finish
                while (_currentPlayer.IsPlaying)
                {
                    await Task.Delay(100);
                }

                _currentPlayer.Dispose();
                _currentPlayer = null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error playing audio '{audioFileName}': {ex.Message}");
            }
        }
    }
}
