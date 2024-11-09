using HellTrail.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Render
{
    public static class Renderer
    {
        public const int PreferedWidth = 320;
        public const int PreferedHeight = 180;
        public const int UIPreferedWidth = 1280;
        public const int UIPreferedHeight = 720;

        public static RenderTarget2D MainTarget { get; set; }
        public static RenderTarget2D UITarget { get; set; }

        public static void Load(GraphicsDevice device)
        {
            MainTarget = new RenderTarget2D(device, PreferedWidth, PreferedHeight);
            UITarget = new RenderTarget2D(device, UIPreferedWidth, UIPreferedHeight);
        }

        public static void Unload()
        {
            MainTarget.Dispose();
            UITarget.Dispose();
        }

        public static void DrawRect(SpriteBatch sb, Vector2 position, Vector2 size, int thickness, Color color, float depth = 0f, float rotation = 0f, Vector2 origin = default)
        {
            var tex = AssetManager.Textures["Pixel"];
            sb.Draw(tex, position, null, color, rotation, origin, size, SpriteEffects.None, depth);
        }

        public static void StartSpriteBatch(SpriteBatch spriteBatch, bool ignoreCam = false, BlendState blend = null, DepthStencilState stencil = null, SamplerState state = null)
        {
            if (state == null)
                state = SamplerState.PointClamp;

            if (stencil == null)
                stencil = DepthStencilState.Default;

            if(blend == null)
                blend = BlendState.AlphaBlend;

            Camera cam = CameraManager.GetCamera;

            if (cam != null && !ignoreCam)
                spriteBatch.Begin(SpriteSortMode.FrontToBack, blend, state, stencil, RasterizerState.CullNone, null, cam.transform);
            else
                spriteBatch.Begin(SpriteSortMode.FrontToBack, blend, state, stencil, RasterizerState.CullNone, null);
        }

        public static void DrawBorderedString(this SpriteBatch sb, SpriteFont font, string text, Vector2 position, Color color, Color borderColor, float rotation, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects, float depth)
        {
            sb.DrawString(font, text, position + Vector2.UnitX * 2, borderColor, rotation, origin, scale, spriteEffects, depth);
            sb.DrawString(font, text, position - Vector2.UnitX * 2, borderColor, rotation, origin, scale, spriteEffects, depth);
            sb.DrawString(font, text, position - Vector2.UnitY * 2, borderColor, rotation, origin, scale, spriteEffects, depth);
            sb.DrawString(font, text, position + Vector2.UnitY * 2, borderColor, rotation, origin, scale, spriteEffects, depth);
            sb.DrawString(font, text, position, color, rotation, origin, scale, spriteEffects, depth);
        }
    }
}
