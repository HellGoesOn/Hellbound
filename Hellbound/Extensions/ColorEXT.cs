using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace Casull.Extensions
{
    public static class ColorEXT
    {
        public static Color AsSolid(this Color c)
        {
            return new Color(c.R, c.G, c.B, 255);
        }

        // very awful fix that only approximates correcty alpha.
        // sadly nothing i can do about it while still using spriteBatch
        public static Color ShaderFix(this Color c, bool fullBright)
        {
            if (fullBright)
                return c.ShaderFix(ShaderParam.FullBright);

            return c.ShaderFix(ShaderParam.None);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ShaderFix(this Color c, ShaderParam param)
        {
            float divisionComponent = (c.A / 255.0f);
            int multiplicationResult = (int)(divisionComponent * Assets.MF_TOLERANCE);

            byte scaled = (byte)Math.Clamp(multiplicationResult, 1, 255);

            byte finalAlpha = scaled;

            if (param == ShaderParam.FullBright)
                finalAlpha = 255;

            return new Color(c.R, c.G, c.B, finalAlpha);
        }
    }

    public enum ShaderParam
    {
        None,
        FullBright
    }
}
