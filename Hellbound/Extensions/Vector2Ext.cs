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
        public static Vector2 FromString(this Vector2 v, string input)
        {
            string[] str = Regex.Replace(input, "[{X:Y:}]", "").Split(" ");
            float x = float.Parse(str[0]);
            float y = float.Parse(str[1]);
            return new(x, y);
        }

        public static Vector2 FromString(string input)
        {
            string[] str = Regex.Replace(input, "[{X:Y:}]", "").Split(" ");
            float x = float.Parse(str[0]);
            float y = float.Parse(str[1]);
            return new(x, y);
        }

        public static bool TryParse(string input, out Vector2 vector)
        {
            string[] str = Regex.Replace(input, "[{X:Y:}]", "").Split(" ");

            if (str.Length < 2 || !float.TryParse(str[0], out float x) || !float.TryParse(str[1], out float y))
            {
                vector = default;
                return false;
            }

            vector = new Vector2(x, y);

            return true;
        }

        public static Vector2 ToInt(this Vector2 v)
        {
            return new Vector2((int)v.X, (int)v.Y);
        }
    }

}
