using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HellTrail.Extensions
{
    public static class StringExt
    {
        public static string Splice(this string s, int length)
        {
            return Regex.Replace(s, "(.{" + length + "})" + ' ', "$1" + Environment.NewLine);
        }

        public static Vector2 ToVector2(this string s)
        {
            return Vector2Ext.FromString(s);
        }

        public static bool TryVector2(this string s, out Vector2 result)
        {
            return Vector2Ext.TryParse(s, out result);
        }
    }
}
