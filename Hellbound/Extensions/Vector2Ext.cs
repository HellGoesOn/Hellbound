using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HellTrail.Extensions
{
    public static class Vector2Ext
    {
        public static Vector2 FromString(string input)
        {
            string[] str = Regex.Replace(input, "[{X:Y:}]", "").Split(" ");
            float x = float.Parse(str[0]);
            float y = float.Parse(str[1]);
            return new(x, y);
        }

        public static Vector2 ToInt(this Vector2 v)
        {
            return new Vector2((int)MathF.Round(v.X), (int)MathF.Round(v.Y));
        }

        public static Vector2 SafeNormalize(this Vector2 v, Vector2 defaultValue = default)
        {
            if(v == Vector2.Zero || float.IsNaN(v.X)  || float.IsNaN(v.Y))
                return defaultValue;

            return Vector2.Normalize(v);
        }
    }

}
