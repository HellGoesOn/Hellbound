using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Casull
{
    public static class SoundEngine
    {
        private static bool _isLooping;
        private static bool _updatedSong;
        public static bool IsMusicPlaying { get; private set; }
        private static string _currentSong;
        public static string DefaultSong = "ChangingSeasons";
        private static int _stoppedFor;
        internal static bool doNotRestart;

        internal static float hiddenVolumeTarget;
        internal static float hiddenVolumeSpeed = 0.1f;
        internal static float hiddenMusicVolumeTarget;
        internal static float hiddenMusicVolumeSpeed = 0.1f;

        internal static float hiddenVolume = 1.0f;
        internal static float hiddenMusicVolume = 1.0f;

        public static void StartMusic(string name, bool loop)
        {
            _updatedSong = true;
            _isLooping = loop;
            _currentSong = name;

            MediaPlayer.Pause();
            IsMusicPlaying = true;
        }

        public static void StopMusic(int stopFor = 0, bool doNotRestart = false)
        {
            SoundEngine.doNotRestart = doNotRestart;
            _stoppedFor = stopFor;
            MediaPlayer.Stop();
            _isLooping = false;
            IsMusicPlaying = false;
        }

        public static void SetTargetVolume(float volume)
        {
            hiddenVolumeTarget = volume;
        } 

        public static void SetTargetMusicVolume(float volume)
        {
            hiddenMusicVolumeTarget = volume;
        }

        private static void UpdateVolume(ref float currentVolume, float targetVolume, float volumeSpeed)
        {
            if (currentVolume != targetVolume) {
                currentVolume += volumeSpeed * Math.Sign(targetVolume - currentVolume);

                if (Math.Abs(targetVolume - currentVolume) <= volumeSpeed) {
                    currentVolume = targetVolume;
                }
            }
        }

        public static void Update()
        {
            UpdateVolume(ref hiddenVolume, hiddenVolumeTarget, hiddenVolumeSpeed);
            UpdateVolume(ref hiddenMusicVolume, hiddenMusicVolumeTarget, hiddenMusicVolumeSpeed);

            MediaPlayer.Volume = GameOptions.MusicVolume * hiddenMusicVolume;

            if (_stoppedFor > 0)
                _stoppedFor--;

            if (_updatedSong) {
                _updatedSong = false;
                MediaPlayer.Stop();
                var song = Assets.GetSong(_currentSong);

                if (song != null)
                    MediaPlayer.Play(song);
            }

            if (MediaPlayer.State == MediaState.Stopped) {
                if (_isLooping) {
                    var song = Assets.GetSong(_currentSong);

                    if (song != null)
                        MediaPlayer.Play(song);
                }
                else if (_stoppedFor <= 0 && !doNotRestart) {
                    StartMusic(DefaultSong, false);
                }
            }
        }

        public static SoundEffectInstance PlaySound(string name, float volume = -1f)
        {
            if (volume == -1f)
                volume = GameOptions.GeneralVolume * hiddenVolume;
            else
                volume = GameOptions.GeneralVolume * volume * hiddenVolume;

            var fx = Assets.GetSound(name)?.CreateInstance();

            if (fx != null) {
                fx.Volume = volume;
                fx.Play();
                return fx;
            }

            return null;
        }
    }
}
