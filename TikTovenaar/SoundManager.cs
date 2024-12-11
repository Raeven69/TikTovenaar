using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace TikTovenaar
{
    public static class SoundManager
    {
        private static MediaPlayer _backgroundMediaPlayer = new MediaPlayer();
        private static MediaPlayer _soundEffectMediaPlayer = new MediaPlayer();

        public static void PlayBackgroundSound(string soundFilePath)
        {
            try
            {
                _backgroundMediaPlayer.Open(new Uri(soundFilePath, UriKind.RelativeOrAbsolute));
                _backgroundMediaPlayer.MediaEnded += (s, e) => _backgroundMediaPlayer.Position = TimeSpan.Zero; // Loop sound
                _backgroundMediaPlayer.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing background sound: {ex.Message}");
            }
        }

        public static void SetBackgroundVulume(int volume)
        {
            _backgroundMediaPlayer.Volume = (double)volume/100;
        }

        public static void StopBackgroundSound()
        {
            _backgroundMediaPlayer.Stop();
        }

        public static void PlaySoundEffect(string soundEffectFilePath)
        {
            try
            {
                _soundEffectMediaPlayer.Open(new Uri(soundEffectFilePath, UriKind.RelativeOrAbsolute));
                _soundEffectMediaPlayer.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing sound effect: {ex.Message}");
            }
        }
    }
}
