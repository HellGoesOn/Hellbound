using Microsoft.Xna.Framework;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Casull.Extensions
{
    public static class Vector2Ext
    {
        public static Vector2 FromString(string input)
        {
            string[] str = Regex.Replace(input, "[{X:Y:}]", "").Split(" ");
            float x = float.Parse(str[0], CultureInfo.InvariantCulture);
            float y = float.Parse(str[1], CultureInfo.InvariantCulture);
            return new(x, y);
        }

        public static Vector2 ToInt(this Vector2 v)
        {
            return new Vector2((int)MathF.Round(v.X), (int)MathF.Round(v.Y));
        }

        public static Vector2 Floor(this Vector2 v) => new Vector2(MathF.Floor(v.X), MathF.Floor(v.Y));

        public static Vector2 SafeNormalize(this Vector2 v, Vector2 defaultValue = default)
        {
            if (v == Vector2.Zero || float.IsNaN(v.X) || float.IsNaN(v.Y))
                return defaultValue;

            return Vector2.Normalize(v);
        }

        public static float ToRotation(this Vector2 v) => (float)Math.Atan2(v.Y, v.X);

        public static Vector2 RotateBy(this Vector2 v, float rotation)
        {
            float xx = v.X * (float)Math.Cos(rotation) - v.Y * (float)Math.Sin(rotation);
            float yy = v.X * (float)Math.Sin(rotation) + v.Y * (float)Math.Cos(rotation);

            return new Vector2(xx, yy);
        }
    }

}
