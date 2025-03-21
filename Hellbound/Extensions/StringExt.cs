﻿using HellTrail.Core;
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

        public static bool TryFrameData(this string s, out FrameData? frameData)
        {
            return FrameData.TryParse(s, out frameData);
        }

        public static bool TryColor(this string s, out Color? color)
        {
            string[] values = Regex.Replace(s, "[^0-9,. ]", "").Split(" ");
            if (!int.TryParse(values[0], out int R)
                || !int.TryParse(values[1], out int G)
                || !int.TryParse(values[2], out int B)
                || !int.TryParse(values[3], out int A))
                {
                color = null;

                return false;
            }

            color = new Color(R, G, B, A);

            return true;
        }
    }
}
