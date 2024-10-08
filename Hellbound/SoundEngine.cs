using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail
{
    public static class SoundEngine
    {
        private static bool _isLooping;
        private static bool _updatedSong;
        private static string _currentSong;

        public static void StartMusic(string name, bool loop)
        {
            _updatedSong = true;
            _isLooping = true;
            _currentSong = name;
        }

        public static void StopMusic()
        {
            MediaPlayer.Stop();
            _isLooping = false;
        }

        public static void Update()
        {
            if (_updatedSong)
            {
                _updatedSong = false;
                MediaPlayer.Stop();
                var song = AssetManager.GetSong(_currentSong);

                if (song != null)
                    MediaPlayer.Play(song);
            }

            if (MediaPlayer.State == MediaState.Stopped)
            {
                if (_isLooping)
                {
                    var song = AssetManager.GetSong(_currentSong);

                    if (song != null)
                        MediaPlayer.Play(song);
                }
            }

            MediaPlayer.Volume = GameOptions.Volume;
        }

        public static SoundEffectInstance PlaySound(string name, float volume = -1f)
        {
            if (volume == -1f)
                volume = GameOptions.Volume;

            var fx = AssetManager.GetSound(name)?.CreateInstance();

            if (fx != null)
            {
                fx.Volume = volume;
                fx.Play();
                return fx;
            }

            return null;
        }
    }
}
