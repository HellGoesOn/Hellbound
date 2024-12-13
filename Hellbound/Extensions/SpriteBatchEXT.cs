﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Extensions
{
    public static class SpriteBatchEXT
    {
        public static void DrawFixed(this SpriteBatch sb, Texture2D texture,
            Vector2 position,
            Rectangle? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            SpriteEffects effects,
            float layerDepth,
            ShaderParam param = ShaderParam.None)
        {
            sb.Draw(texture, position, sourceRectangle, color.ShaderFix(param), rotation, origin, scale, effects, layerDepth);
        }
    }
}
