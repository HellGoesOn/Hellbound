using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Casull.Render
{
    public struct MarkdownText
    {
        public MarkdownText(string text)
        {
            string[] lines = text.Split(new string[] { Environment.NewLine, "\n", "\r"}, StringSplitOptions.None);

            _lines = [];

            foreach(string line in lines) {
                _lines.Add(new(line));
            }
        }

        readonly List<MarkDownLine> _lines;

        public static implicit operator MarkdownText(string value)
        {
            return new MarkdownText(value);   
        }

        public void Draw(IMarkdownTextDrawer drawer, Vector2 position, SpriteFont font, float rotation = 0f, Vector2? origin = null, Color? color = null, Vector2? scale = null)
        {
            float y = 0;
            foreach (var line in _lines) {

                float x = 0;
                for (int i = 0; i < line.pairs.Count; i++) {
                    var pair = line.pairs[i];

                    color ??= Color.White;

                    scale ??= Vector2.One;

                    origin ??= Vector2.Zero;

                    if (i > 0)
                        x += font.MeasureString(line.pairs[i - 1].text).X * scale.Value.X;

                    drawer.Draw(font, pair.text, position + new Vector2(x, y), pair.color == Color.White ? color.Value : pair.color, rotation, origin.Value, (Vector2)scale, SpriteEffects.None, 1f);
                }

                y += font.MeasureString("Y").Y * scale.Value.Y;
            }
        }

        public readonly string GetRaw()
        {
            string input = string.Join("\n", _lines.Select(x => string.Join("", x.pairs.Select(x => x.text))));

            // Regular expression to match the pattern <FF00FF/raw>
            string pattern = @"<FF00FF/(.*?)>";

            // Replace the matched pattern with just the content inside
            string result = Regex.Replace(input, pattern, "$1");

            return result;

        }

        public void Draw(IMarkdownTextDrawer drawer, int totalLength, Vector2 position, SpriteFont font, float rotation = 0f, Vector2? origin = null, Color? color = null, Vector2? scale = null, object[] additionalArgs = null)
        {
            float y = 0;
            foreach (var line in _lines) {
                float x = 0;
                int localLength = line.pairs.Select(x=>x.text.Length).Sum();
                for (int i = 0; i < line.pairs.Count; i++) {
                    var pair = line.pairs[i];

                    color ??= Color.White;

                    scale ??= Vector2.One;

                    origin ??= Vector2.Zero;

                    if (i > 0)
                        x += font.MeasureString(line.pairs[i - 1].text).X * scale.Value.X;

                    drawer.Draw(font, pair.text, position + new Vector2(x, y), pair.color == Color.White ? color.Value : pair.color, rotation, origin.Value, (Vector2)scale, SpriteEffects.None, 1f);
                }

                y += font.MeasureString("Y").Y * scale.Value.Y;
            }
        }
    }

    public static class Markdown
    {
        static BorderedText _bordered;
        public static BorderedText Bordered {
            get {
                _bordered ??= new BorderedText();

                return _bordered;
            }
        }

        static FlatText _flat;
        public static FlatText Flat {
            get {
                _flat ??= new FlatText();

                return _flat;
            }
        }

        public class BorderedText : IMarkdownTextDrawer
        {
            public Color borderColor = Color.Black;
            public float opacity;

            public void Draw(SpriteFont font, string text,  Vector2 pos, Color clr, float rot, Vector2 orig, Vector2 scale, SpriteEffects sfx, float depth)
            {
                var sb = Main.instance.spriteBatch;
                sb.DrawBorderedString(font, text, pos, clr * opacity, borderColor * opacity, rot, orig, scale, sfx, depth);
            }
        }

        public class FlatText : IMarkdownTextDrawer
        {
            public void Draw(SpriteFont font, string text,  Vector2 pos, Color clr, float rot, Vector2 orig, Vector2 scale, SpriteEffects sfx, float depth)
            {
                var sb = Main.instance.spriteBatch;
                sb.DrawString(font, text, pos, clr, rot, orig, scale, sfx, depth);
            }
        }
    }

    public interface IMarkdownTextDrawer
    {
        void Draw(SpriteFont font, string text, Vector2 pos, Color clr, float rot, Vector2 orig, Vector2 scale, SpriteEffects sfx, float depth);
    }
     
    readonly struct MarkDownLine
    {
        public readonly List<StringColorPair> pairs;

        public MarkDownLine(string text)
        {
            // Convert matches to an array of strings
            string[] result = SplitStrings(text);

            pairs = [];

            foreach(string str in result) {
                var clr = Regex.Match(str, @"[0-9A-Fa-f]{6}").Value;
                var val = str;
                Color stringColor = Color.White;
                if (!string.IsNullOrWhiteSpace(clr) && clr.Length == 6) {

                    int r = int.Parse(clr[..2], NumberStyles.HexNumber);
                    int g = int.Parse(clr.Substring(2, 2), NumberStyles.HexNumber);
                    int b = int.Parse(clr.Substring(4, 2), NumberStyles.HexNumber);

                    stringColor = new Color(r, g, b);
                    val = Regex.Match(str, @"\/(.*)\>").Groups[1].Value;
                }

                pairs.Add(new StringColorPair(val, stringColor));
            }
        }

        public static string[] SplitStrings(string input)
        {
            var output = input;

            int bracketCount = 0;

            List<int> indecies = [];

            for (int i = 0; i < output.Length; i++) {

                var c = output[i];

                if (c == '<') {
                    if (bracketCount <= 0)
                        indecies.Add(i);
                    bracketCount++;
                }
                else if (c == '>') {
                    if (bracketCount > 0) {
                        bracketCount--;

                        if(bracketCount <= 0)
                            indecies.Add(i+1);
                    }
                }

            }

            int offsetFix = 0;
            foreach(int index in indecies) {
                output = output.Insert(index+offsetFix, "\t");
                offsetFix++;
            }

            return output.Split('\t');
        }

    }

    readonly struct StringColorPair(string text, Color color)
    {
        public readonly string text = text;
        public readonly Color color = color;
    }
}
