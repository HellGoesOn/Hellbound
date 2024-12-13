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

        public static void Update()
        {
            MediaPlayer.Volume = GameOptions.MusicVolume;

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
                volume = GameOptions.GeneralVolume;
            else
                volume = GameOptions.GeneralVolume * volume;

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
