using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using SpriteFontPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail
{
    public static class Assets
    {
        public static Dictionary<string, Texture2D> Textures = new();
        private readonly static Dictionary<string, Song> _songs = new();
        private readonly static Dictionary<string, SoundEffect> _sounds = new();

        public static SpriteFont Arial;
        public static SpriteFont DefaultFont;
        public static SpriteFont CombatMenuFont;
        public static SpriteFont SmallFont;

        public static void Load(Main main)
        {
            string[] textures = Directory.GetFiles("Assets/Textures/", "*.png", SearchOption.AllDirectories);
            string[] songs = Directory.GetFiles("Assets/Audio/Music");
            string[] sounds = Directory.GetFiles("Assets/Audio/Sounds");

            Textures.Clear();

            foreach (string song in songs)
            {
                var songPath = Path.GetFileNameWithoutExtension(song);
                Song resource = main.Content.Load<Song>("Assets/Audio/Music/" + songPath);
                _songs.Add(songPath, resource);
            }

            foreach (string sound in sounds)
            {
                var soundPath = Path.GetFileNameWithoutExtension(sound);
                SoundEffect resource = main.Content.Load<SoundEffect>("Assets/Audio/Sounds/" + soundPath);
                _sounds.Add(soundPath, resource);
            }

            foreach (string texture in textures)
            {
                var texturePath = Path.GetFileNameWithoutExtension(texture);
                LoadTexture(texturePath, texture, main);
            }
            var pixel = new Texture2D(main.GraphicsDevice, 1, 1);
            pixel.SetData(new Color[1] { Color.White });
            Textures.Add("Pixel", pixel);

            var fontName = "alphbeta";

            //var fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(@"C:\\Windows\\Fonts\arial.ttf"),

            var arial = TtfFontBaker.Bake(File.ReadAllBytes(@"C:\\Windows\\Fonts\arial.ttf"),
                24,
                1024,
                1024,
                new[]
                {
                    CharacterRange.BasicLatin,
                    CharacterRange.Latin1Supplement,
                    CharacterRange.LatinExtendedA,
                    CharacterRange.Cyrillic
                }
            );
            var fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(Environment.CurrentDirectory + $"\\Assets\\{fontName}.ttf"),
                24,
                1024,
                1024,
                new[]
                {
                    CharacterRange.BasicLatin,
                    CharacterRange.Latin1Supplement,
                    CharacterRange.LatinExtendedA,
                    CharacterRange.Cyrillic
                }
            );

            DefaultFont = fontBakeResult.CreateSpriteFont(main.GraphicsDevice);
            
            fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(Environment.CurrentDirectory + $"\\Assets\\{fontName}.ttf"),
                30,
                1024,
                1024,
                new[]
                {
                    CharacterRange.BasicLatin,
                    CharacterRange.Latin1Supplement,
                    CharacterRange.LatinExtendedA,
                    CharacterRange.Cyrillic
                }
            );

            CombatMenuFont = fontBakeResult.CreateSpriteFont(main.GraphicsDevice);

            fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(Environment.CurrentDirectory + $"\\Assets\\{fontName}.ttf"),
                16,
                1024,
                1024,
                new[]
                {
                    CharacterRange.BasicLatin,
                    CharacterRange.Latin1Supplement,
                    CharacterRange.LatinExtendedA,
                    CharacterRange.Cyrillic
                }
            );

            SmallFont = fontBakeResult.CreateSpriteFont(main.GraphicsDevice);

            Arial = arial.CreateSpriteFont(main.GraphicsDevice);
        }

        public static void Unload()
        {
            foreach(Texture2D tex in Textures.Values)
            {
                tex.Dispose();
            }

            foreach (var song in _songs)
            {
                song.Value.Dispose();
            }

            foreach (var sound in _sounds)
            {
                sound.Value.Dispose();
            }

            _songs.Clear();
            _sounds.Clear();
            Textures.Clear();
        }

        public static Texture2D LoadTexture(string id, string path, Main main)
        {
            FileStream str = new(path, FileMode.Open);
            Texture2D loadedTexture = Texture2D.FromStream(main.GraphicsDevice, str);

            str.Dispose();

            if (loadedTexture != null)
            {
                Textures.Add(id, loadedTexture);
                return loadedTexture;
            }

            throw new Exception("There has been an issue with loading asset: " + path);
        }

        public static Song GetSong(string v)
        {
            if (_songs.TryGetValue(v, out Song r))
            {
                return r;
            }

            return null;
        }

        public static SoundEffect GetSound(string id)
        {
            if (_sounds.TryGetValue(id, out SoundEffect result))
            {
                return result;
            }

            return null;
        }

        public static Texture2D GetTexture(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Textures["Pixel"];
            if(Textures.TryGetValue(id, out Texture2D texture))
            {
                return texture;
            }
            return Textures["Pixel"];
        }
    }
}
