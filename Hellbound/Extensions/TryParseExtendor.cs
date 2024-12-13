using Casull.Core.ECS.Components;
using Microsoft.Xna.Framework;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Casull.Extensions
{
    public static class TryParseExtendor
    {
        public static bool TryParse(string input, out Vector2 vector)
        {
            string[] str = Regex.Replace(input, "[{X:Y:}]", "").Split(" ");

            if (str.Length < 2 || !float.TryParse(str[0], CultureInfo.InvariantCulture, out float x) || !float.TryParse(str[1], CultureInfo.InvariantCulture, out float y)) {
                vector = default;
                return false;
            }

            vector = new Vector2(x, y);

            return true;
        }

        public static Frame FromString(this Frame frame, string input)
        {
            if (Frame.TryParse(input, ref frame)) {
                return frame;
            }
            return new Frame(0, 0, 0, 0, 0);
        }

        public static Vector2 FromString(this Vector2 v, string input)
        {
            string splicedString = Regex.Replace(input, "[{X:Y:}]", "");
            string[] str = splicedString.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (str.Length < 2
                || !float.TryParse(str[0], CultureInfo.InvariantCulture, out float x)
                || !float.TryParse(str[1], CultureInfo.InvariantCulture, out float y)) {
                return default;
            }
            return new(x, y);
        }

        public static Animation FromString(this Animation frame, string input)
        {
            if (Animation.TryParse(input, ref frame)) {
                return frame;
            }
            return new Animation(0, 0);
        }

        public static bool TryParseColor(string input, out Color color)
        {
            string[] values = Regex.Replace(input, "[^0-9,. ]", "").Split(" ");
            if (!int.TryParse(values[0], out int R)
                || !int.TryParse(values[1], out int G)
                || !int.TryParse(values[2], out int B)
                || !int.TryParse(values[3], out int A)) {
                color = default(Color);
                return false;
            }
            color = new(R, G, B, A);
            return true;
        }

        public static Color FromString(this Color v, string input)
        {
            string[] values = Regex.Replace(input, "[^0-9,. ]", "").Split(" ");
            if (values.Length < 4 || !int.TryParse(values[0], out int R)
                || !int.TryParse(values[1], out int G)
                || !int.TryParse(values[2], out int B)
                || !int.TryParse(values[3], out int A)) {
                return default;
            }

            return new Color(R, G, B, A);
        }
    }
}
